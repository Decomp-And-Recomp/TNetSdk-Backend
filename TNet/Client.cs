using System.Collections.Concurrent;
using System.Net.Sockets;
using TNet.Helpers;

namespace TNet;

internal class Client
{
    public readonly TcpClient client;
    public readonly NetworkStream stream;

    public readonly DateTime connectTime;

    public bool disconnected { get; private set; }

    public bool loggedIn;

    public ushort id;

    public string nickname = string.Empty;

    public Room? room;

    public float heartValue;

    public readonly ConcurrentDictionary<ushort, byte[]> userVariables = [];

    public Client(TcpClient client)
    {
        this.client = client;
        stream = client.GetStream();

        connectTime = DateTime.UtcNow;

        _ = HeartbeatLoop();
    }

    async Task HeartbeatLoop()
    {
        while (!disconnected)
        {
            await Task.Delay(1000);

            heartValue += 1;

            if (heartValue < Variables.heartbeatTimeout) continue;

            Disconnect(DisconnectCode.HeartbeatTimeout);
        }
    }

    public async Task Send(byte[] data)
    {
        await stream.WriteAsync(data);
    }

    public void Disconnect(DisconnectCode code)
    {
        if (disconnected) return;
        room?.Disconnect(this);

        client.Close();
        client.Dispose();

        disconnected = true;

        Logger.Info($"Client '{id}' disconnected, reason: '{code}'");
    }
}
