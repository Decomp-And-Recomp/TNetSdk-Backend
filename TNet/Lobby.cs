using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using TNet.External;
using TNet.Helpers;
using TNet.Protocols;

namespace TNet;

internal static class Lobby
{
    public const int MaxPacketLength = 4098;
    public const int HeaderSize = 10;

    private static TcpListener Listener = null!;

    public static ConcurrentDictionary<ushort, Client> Clients = [];
    public static ConcurrentDictionary<ushort, Room> Rooms = [];

    private static readonly ProtocolHandler[] Handlers = [
        new DummyProtocolHandler(),
        new Protocols.SystemProtocol.SystemProtocolHandler(),
        new Protocols.RoomProtocol.RoomProtocolHandler()
    ];

    public static async Task Run(int port)
    {
        Listener = new(IPAddress.Any, port);

        Listener.Start();

        Logger.Info($"Server now running on port '{port}'");

        await ServerLoop();
    }

    public static async Task ServerLoop()
    {
        while (true)
        {
            _ = HandleClient(await Listener.AcceptTcpClientAsync());
        }
    }

    static async Task HandleClient(TcpClient tcpClient)
    {
        if (Clients.Count >= Variables.MaxClients)
        {
            tcpClient.Close();
            return;
        }

        Client client = new(tcpClient);

        while (true)
        {
            client.id = RandomHelper.GetClientId();

            if (Clients.TryAdd(client.id, client)) break;
        }

        Logger.Info($"New client connected.");

        byte[] buffer = new byte[MaxPacketLength];
        List<byte> received = [];

        int read;
        int length = 0;
        bool lengthCalculated = false;

        while (true)
        {
            try
            {
                read = await client.stream.ReadAsync(buffer);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                client.Disconnect(DisconnectCode.ReadException);
                return;
            }

            if (read == 0)
            {
                client.Disconnect(DisconnectCode.SocketDisconnect);
                return;
            }

            received.AddRange(buffer.Take(read));

            while (received.Count > HeaderSize)
            {
                if (lengthCalculated)
                {
                    if (received.Count < length) break;
                }
                else
                {
                    EncrpytionHelper.Decrypt(received); // decrypt here and once so we dont do that any more.

                    length = GetLength(received);

                    if (length < HeaderSize || length > MaxPacketLength)
                    {
                        Logger.Error($"Length received: {length}");
                        client.Disconnect(DisconnectCode.DataOutOfBounds);
                        return;
                    }

                    lengthCalculated = true;
                    continue;
                }

                var takenData = received.Take(length).ToArray();
                received.RemoveRange(0, length);

                lengthCalculated = false;

                try
                {
                    HandlePacket(client, takenData);
                }
                catch (Exception ex)
                {
                    client.Disconnect(DisconnectCode.PacketException);
                    Logger.Exception(ex);
                    return;
                }
            }
        }
    }

    static int GetLength(List<byte> data)
    {
        return ((data[0] << 8) | data[1]);
    }

    static void HandlePacket(Client client, byte[] data)
    {
        UnPacker unPacker = new();
        unPacker.SetData(data);

        if (!unPacker.Initialize()) throw new Exception("Cannot initialize unPacker.");

        Handlers[(int)unPacker.protocol].Handle(client, unPacker);
    }
}
