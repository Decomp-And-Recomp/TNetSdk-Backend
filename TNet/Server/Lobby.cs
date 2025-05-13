using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            case LobbyState.Initing: Debug.LogError("Tried running server while Initing one."); return;
            case LobbyState.Running: Debug.LogError("Tried running server while one is already Running."); return;
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

        LobbyUtils.Log($"Lobby now running on: {address}:{port}", ConsoleColor.Green);

        await ServerLoop();
    }

    static async Task ServerLoop()
    {
        if (listener == null) return; // shouldnt happen but the warning was so annoying.

        while (true)
        {
            _ = HandleConnection(await listener.AcceptTcpClientAsync());
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
            LobbyUtils.Log("Unable to add new client, disconnecting.", ConsoleColor.Red);
            DisconnectClient(client, DisconnectCode.CouldntAddToDictionary);
            return;
        }

        LobbyUtils.LogNewConnection(client);

        NetworkStream stream = tcpClient.GetStream();

        byte[] buffer = new byte[maxDataLength];

        int read;

        while (true)
        {
            read = await stream.ReadAsync(buffer);

            if (read == 0)
            {
                DisconnectClient(client);
                break;
            }
            else if (read > maxDataLength)
            {
                DisconnectClient(client, DisconnectCode.TooMuchData);
                break;
            }

            OnReceive(buffer[..read], client);
        }
    }

    static void OnReceive(byte[] bytes, Client client)
    {
        UnPacker unPacker = new();
        
        Packet p = new(bytes, bytes.Length, true);

        LobbyUtils.Decrypt(p, blowFish);

        if (!unPacker.ParserPacket(p)) return;

        if (unPacker.GetProtocol() > 2 || unPacker.GetProtocol() < 1)
        {
            DisconnectClient(client, DisconnectCode.UnknownProtocol);
            return;
        }


        LobbyUtils.Log($"Protocol-{unPacker.GetProtocol()} Cmd-{unPacker.GetCmd()}", ConsoleColor.Cyan);

        if (unPacker.GetProtocol() == 1)
        {
            SysCMD sysCommand = (SysCMD)unPacker.GetCmd();

            switch (sysCommand) // sys commands
            {
                case SysCMD.heartbeat: _ = LobbyCmdImpl.OnSystemHeartbeat(unPacker, client); return; // CMD.sys_heartbeat
                case SysCMD.login: _ = LobbyCmdImpl.OnSystemPlayerLogin(unPacker, client); return;
            }

            LobbyUtils.LogUnimpl(sysCommand);
            return;
        }

        RoomCMD command = (RoomCMD)unPacker.GetCmd();

        switch (command) // room commands
        {
            case RoomCMD.drag_list: _ = LobbyCmdImpl.OnRoomDragList(unPacker, client); return;
            case RoomCMD.create: _ = LobbyCmdImpl.OnRoomCreate(unPacker, client); return;
            case RoomCMD.leave: LobbyCmdImpl.OnRoomLeave(client); return;
            case RoomCMD.set_var: _ = LobbyCmdImpl.OnRoomSetVar(unPacker, client); return;
            case RoomCMD.broadcast_msg: _ = LobbyCmdImpl.OnRoomBroadcastMsg(unPacker, client); return;
        }

        LobbyUtils.LogUnimpl(command);
    }

    public static void DisconnectClient(Client client)
    {
        DisconnectClient(client, DisconnectCode.NoCode);
    }

    public static void DisconnectClient(Client client, DisconnectCode code)
    {
        LobbyUtils.Log("Disconnected player: " + code);
        if (code == DisconnectCode.SuspiciousRequests) Debug.LogStackFull(ConsoleColor.DarkMagenta);
        client.Disconnect();
    }
}