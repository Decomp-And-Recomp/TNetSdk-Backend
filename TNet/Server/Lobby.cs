using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using TNet.Encryption;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Cmd;

namespace TNet.Server;

public enum LobbyState { NotRunning, Initing, Running}

internal static class Lobby
{
    const int maxDataLength = 4096;

    const string key = "Triniti_Tlck";
    public static readonly BlowFish blowFish = new(key);
    static TcpListener? listener;

    public readonly static ConcurrentDictionary<ushort, Client> clients = [];
    public readonly static ConcurrentDictionary<ushort, Room> rooms = [];

    public static LobbyState state { get; private set; } = LobbyState.NotRunning;

    public static async Task Run(IPAddress address, int port)
    {
        switch (state)
        {
            case LobbyState.Initing: Debug.LogError("Tried running server while Initing."); return;
            case LobbyState.Running: Debug.LogError("Tried running server while is already Running."); return;
        }

        state = LobbyState.Initing;

        clients.Clear();
        rooms.Clear();

        try
        {
            listener = new(address, port);
            listener.Start();
        }
        catch (Exception ex)
        {
            Debug.LogException("Exception on running the server.", ex);
            state = LobbyState.NotRunning;
            return;
        }

        Debug.Log($"Lobby now running on port {port}", ConsoleColor.Green);

        await ServerLoop();
    }

    static async Task ServerLoop()
    {
        if (listener == null) return; // shouldnt happen but the warning was so annoying.

        while (true)
        {
            try
            {
                _ = HandleConnection(await listener.AcceptTcpClientAsync());
            }
            catch (Exception ex)
            {
                Debug.LogException("SERVER LOOP ALMOST BROKE", ex);
            }
        }
    }

    static async Task HandleConnection(TcpClient tcpClient)
    {
        Client client = new(tcpClient);

        bool added = false;

        for (ushort i = 0; i < ushort.MaxValue; i++)
        {
            if (!clients.TryAdd(i, client)) continue;

            added = true;

            client.id = i;
            break;
        }

        if (!added)
        {
            Debug.Log("Unable to add new client, disconnecting.", ConsoleColor.Red);
            DisconnectClient(client, DisconnectCode.CouldntAddToDictionary);
            return;
        }

        LobbyUtils.LogNewConnection(client);

        NetworkStream stream = tcpClient.GetStream();

        List<byte> recivied = [];

        byte[] buffer = new byte[maxDataLength];
        int read;

        bool lengthCalculated = false;
        ushort length = 5;

        while (true)
        {
            read = await stream.ReadAsync(buffer);

            if (read == 0)
            {
                DisconnectClient(client, DisconnectCode.SocketDisconnect);
                break;
            }
            else if (read > maxDataLength)
            {
                DisconnectClient(client, DisconnectCode.TooMuchData);
                break;
            }

            recivied.AddRange(buffer.Take(read));

            // multi package checking
            while (recivied.Count > Header.HEADER_LENGTH)
            {
                if (!lengthCalculated)
                {
                    lengthCalculated = true;
                    length = GetLength(recivied);

                    if (length > maxDataLength)
                    {
                        Debug.Log("Data amount: " + length);
                        DisconnectClient(client, DisconnectCode.TooMuchData);
                        break;
                    }
                    if (length < Header.HEADER_LENGTH)
                    {
                        Debug.Log("Data amount: " + length);
                        DisconnectClient(client, DisconnectCode.TooShortData);
                        break;
                    }
                }

                if (recivied.Count < length) break;

                var bytes = recivied.GetRange(0, length).ToArray();
                recivied.RemoveRange(0, length);
                _ = Task.Run(() => OnReceive(bytes, client));

                lengthCalculated = false;
            }
        }
    }

    static ushort GetLength(List<byte> data)
    {
        byte[] bytes = data.Take(8).ToArray();

        LobbyUtils.Decrypt(ref bytes);

        return LobbyUtils.WatchUInt16(bytes, 0);
    }

    static void OnReceive(byte[] bytes, Client client)
    {
        UnPacker unPacker = new();
        
        Packet p = new(bytes, bytes.Length, false);

        LobbyUtils.Decrypt(p);

        if (!unPacker.ParserPacket(p))
        {
            Debug.LogWarning("UnPacker couldnt unpack the packet");
            return;
        }

        if (unPacker.GetLength() != bytes.Length) 
            Debug.Log($"unPacker and bytes length doesnt match: {unPacker.GetLength()} - {bytes.Length}");

        if (unPacker.GetProtocol() > 2 || unPacker.GetProtocol() < 1)
        {
            DisconnectClient(client, DisconnectCode.UnknownProtocol);
            return;
        }

        //LobbyUtils.Log($"Protocol-{unPacker.GetProtocol()} Cmd-{unPacker.GetCmd()}", ConsoleColor.Cyan);

        if (unPacker.GetProtocol() == 1)
        {
            SysCMD sysCommand = (SysCMD)unPacker.GetCmd();

            switch (sysCommand) // sys commands
            {
                case SysCMD.heartbeat: _ = LobbyCmdImpl.OnSystemHeartbeat(client); return; // CMD.sys_heartbeat
                case SysCMD.login: _ = LobbyCmdImpl.OnSystemPlayerLogin(unPacker, client); return;
            }

            LobbyUtils.LogUnimpl(sysCommand);
            return;
        }

        RoomCMD command = (RoomCMD)unPacker.GetCmd();

        switch (command) // room commands
        {
            case RoomCMD.drag_list: LobbyCmdImpl.OnRoomDragList(unPacker, client); return;
            case RoomCMD.create: LobbyCmdImpl.OnRoomCreate(unPacker, client); return;
            case RoomCMD.join: LobbyCmdImpl.OnRoomJoin(unPacker, client); return;
            case RoomCMD.leave: LobbyCmdImpl.OnRoomLeave(client); return;
            case RoomCMD.set_var: LobbyCmdImpl.OnRoomSetVar(unPacker, client); return;
            case RoomCMD.set_user_var: LobbyCmdImpl.OnRoomSetUserVar(unPacker, client); return;
            case RoomCMD.broadcast_msg: LobbyCmdImpl.OnRoomBroadcastMsg(unPacker, client); return;
            case RoomCMD.lock_req: LobbyCmdImpl.OnRoomLockReq(unPacker, client); return;
            case RoomCMD.start: LobbyCmdImpl.OnRoomStart(client); return;
        }

        LobbyUtils.LogUnimpl(command);
    }

    /*internal enum TNetEventRoom
    {
        GET_ROOM_LIST = 2,
        ROOM_REMOVE = 7,
        ROOM_CREATION = 4,
        ROOM_JOIN = 9,
        USER_ENTER_ROOM = 10,
        USER_EXIT_ROOM = 12,
        USER_BE_KICKED = 14,
        OBJECT_MESSAGE = 25,
        ROOM_VARIABLES_UPDATE = 18,
        USER_VARIABLES_UPDATE = 20,
        USER_STATE = 22,
        ROOM_NAME_CHANGE = 16,
        LOCK_STH = 27,
        UNLOCK_STH = 29,
        ROOM_START = 31,
        ROOM_MASTER_CHANGE = 32,
        ROOM_REMOVE_RES = 6,
        ROOM_COMMENT_CHANGE = 34
    }*/

    public static void DisconnectClient(Client client, DisconnectCode code)
    {
        if (client.disconnected) return;

        Debug.Log("Disconnected player: " + code);
        if (code == DisconnectCode.SuspiciousRequests)
        {
            Debug.LogStack(ConsoleColor.DarkMagenta);
            return; // Off for testing..
        }
        client.Disconnect();
    }
}