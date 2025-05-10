using System.Net;
using System.Net.Sockets;
using TNet.Encryption;
using TNet.Helpers;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Cmd;

namespace TNet.Server;

public enum LobbyState { NotRunning, Initing, Running}

internal static class Lobby
{
    const int maxDataLength = 1024;

    const string key = "Triniti_Tlck";
    public static readonly BlowFish blowFish = new(key);
    static TcpListener? listener;

    public readonly static List<Client> clients = [];
    public readonly static List<Room> rooms = [];

    public static LobbyState state { get; private set; } = LobbyState.NotRunning;

    public static async Task Run(IPAddress address, int port)
    {
        switch (state)
        {
            case LobbyState.Initing: DebugHelper.LogError("Tried running server while Initing one."); return;
            case LobbyState.Running: DebugHelper.LogError("Tried running server while one is already Running."); return;
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
            DebugHelper.LogException("Exception on running the server.", ex);
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
        clients.Add(client);

        LobbyUtils.LogNewConnection(tcpClient);

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

            await OnReceive(buffer[..read], client);
        }
    }

    static async Task OnReceive(byte[] bytes, Client client)
    {
        UnPacker unPacker = new();
        
        Packet p = new(bytes, bytes.Length, true);

        LobbyUtils.Decrypt(ref p, blowFish);

        if (!unPacker.ParserPacket(p)) Console.WriteLine("S");

        if (unPacker.GetProtocol() > 2 || unPacker.GetProtocol() < 1)
        {
            DisconnectClient(client, DisconnectCode.BadProtocol);
            return;
        }

        CMD command = (CMD)unPacker.GetCmd();

        //LobbyUtils.Log(unPacker.GetCmd() + " Protocol" + unPacker.GetProtocol(), ConsoleColor.Cyan);
        LobbyUtils.Log($"Cmd-{unPacker.GetCmd()} Protocol-{unPacker.GetProtocol()}", ConsoleColor.Cyan);

        if (unPacker.GetProtocol() == 1)
        {
            switch (command) // sys commands
            {
                //case CMD.sys_heartbeat: await LobbyCmdImpl.OnSystemHeartbeat(unPacker, client); return; // CMD.sys_heartbeat
                case CMD.sys_login: await LobbyCmdImpl.OnSystemPlayerLogin(unPacker, client); return;
            }

            LobbyUtils.LogUnimpl(unPacker.GetCmd() + ":" + unPacker.GetProtocol());
            return;
        }

        switch (command) // room commands
        {
            //case CMD.room_create: await LobbyCmdImpl.OnRoomCreate(unPacker, client); return;
            case CMD.room_set_var: await LobbyCmdImpl.OnRoomSetVar(unPacker, client); return;
        }

        LobbyUtils.LogUnimpl(unPacker.GetCmd() + ":" + unPacker.GetProtocol());
    }

    static void DisconnectClient(Client client)
    {
        DisconnectClient(client, DisconnectCode.NoCode);
    }

    static void DisconnectClient(Client client, DisconnectCode code)
    {
        LobbyUtils.Log("Disconnected player: " + code);
        client.Disconnect();
    }
}