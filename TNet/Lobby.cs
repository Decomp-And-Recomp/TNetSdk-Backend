using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using TNet.External;
using TNet.Helpers;
using TNet.Protocols;

namespace TNet;

internal static class Lobby
{
    public const int maxPacketLength = 4098;
    public const int headerSize = 10;

    static TcpListener listener = null!;
    public static readonly BlowFish blowFish = new("ExampleKey");
    public static Version version;

    public static ConcurrentDictionary<ushort, Client> clients = [];
    public static ConcurrentDictionary<ushort, Room> rooms = [];

    static readonly ProtocolHandler[] handlers = {
        new DummyProtocolHandler(),
        new Protocols.Common.CommonProtocolHandler(),
        new Protocols.Room.RoomProtocolHandler()
    };

    public static async Task Run(int port)
    {
        listener = new(IPAddress.Any, port);

        listener.Start();

        Logger.Info($"Server now running on port '{port}'");

        await ServerLoop();
    }

    public static async Task ServerLoop()
    {
        while (true)
        {
            _ = HandleClient(await listener.AcceptTcpClientAsync());
        }
    }

    static async Task HandleClient(TcpClient tcpClient)
    {
        Client client = new(tcpClient);

        Logger.Info($"New client connected.");

        byte[] buffer = new byte[maxPacketLength];
        List<byte> received = [];

        int read;
        int length = 0;
        bool lengthCalculated = false;

        while (true)
        {
            read = await client.stream.ReadAsync(buffer);

            if (read == 0)
            {
                client.Disconnect(DisconnectCode.SocketDisconnect);
                return;
            }

            received.AddRange(buffer.Take(read));

            while (received.Count > headerSize)
            {
                if (lengthCalculated)
                {
                    if (received.Count < length) break;
                }
                else
                {
                    EncrpytionHelper.Decrypt(received); // decrypt here and once so we dont do that any more.

                    length = GetLength(received);

                    if (length < headerSize || length > maxPacketLength)
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
                    Logger.Exception(ex);
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

        if (!unPacker.Initialize())
        {
            Logger.Error("Cannot initialize unPacker.");
            return;
        }

        handlers[(int)unPacker.protocol].Handle(client, unPacker);
    }
}
