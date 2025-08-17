using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server;

public enum LobbyState { NotRunning, Initing, Running}

internal static class Lobby
{
    const int maxDataLength = 4096;

    static TcpListener? listener;

    public readonly static ConcurrentDictionary<ushort, Client> clients = [];
    public readonly static ConcurrentDictionary<ushort, Room> rooms = [];

    public static LobbyState state { get; private set; } = LobbyState.NotRunning;
    public static Game game { get; private set; }

    public static async Task Run(IPAddress address, int port, Game runningFor)
    {
        game = runningFor;

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

        _ = Task.Run(AdminPanel.Run);

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
        if (tcpClient.Client.RemoteEndPoint == null)
        {
            Debug.Log("tcpClient.Client.RemoteEndPoint == null");
            tcpClient.Close();
            return;
        }

        if (BanList.IsBanned(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString()))
        {
            Debug.Log("Blocked banned ip");
            tcpClient.Close();
            return;
        }
        
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
            try
            {
                read = await stream.ReadAsync(buffer);
            }
            catch
            {
                DisconnectClient(client, DisconnectCode.ReadException);
                return;
            }

            if (read == 0)
            {
                DisconnectClient(client, DisconnectCode.SocketDisconnect);
                break;
            }
            if (read > maxDataLength)
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

        //LobbyUtils.Decrypt(ref bytes);

        return LobbyUtils.WatchUInt16(bytes, 0);
    }

    static void OnReceive(byte[] bytes, Client client)
    {
        UnPacker unPacker = new();
        
        Packet p = new(bytes, bytes.Length, false);

        //LobbyUtils.Decrypt(p);

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

        //Debug.Log($"Protocol-{unPacker.GetProtocol()} Cmd-{unPacker.GetCmd()}", ConsoleColor.Cyan);

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
            case RoomCMD.kick_user: LobbyCmdImpl.OnRoomKick(unPacker, client); return;
            case RoomCMD.rename: LobbyCmdImpl.OnRoomRename(unPacker, client); return;
            case RoomCMD.set_var: LobbyCmdImpl.OnRoomSetVar(unPacker, client); return;
            case RoomCMD.set_user_var: LobbyCmdImpl.OnRoomSetUserVar(unPacker, client); return;
            case RoomCMD.send_msg: LobbyCmdImpl.OnRoomSendMsg(unPacker, client); return;
            case RoomCMD.broadcast_msg: LobbyCmdImpl.OnRoomBroadcastMsg(unPacker, client); return;
            case RoomCMD.lock_req: LobbyCmdImpl.OnRoomLockReq(unPacker, client); return;
            case RoomCMD.start: LobbyCmdImpl.OnRoomStart(client); return;
        }

        LobbyUtils.LogUnimpl(command);
    }

    public static void DisconnectClient(Client client, DisconnectCode code)
    {
        if (client.disconnected) return;

        Debug.Log("Disconnected player: " + code);
        if (code == DisconnectCode.SuspiciousRequests)
        {
            Debug.LogStack(ConsoleColor.DarkMagenta);
            return; // Off for testing.. -- its never have been on since
        }
        client.Disconnect();
    }
}