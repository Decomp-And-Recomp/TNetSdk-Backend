using System.Net.Sockets;
using TNet.Helpers;

namespace TNet;

internal class Client
{
    public readonly TcpClient client;
    public readonly NetworkStream stream;
    public bool disconnected { get; private set; }

    public bool loggedIn;

    public ushort id;

    public string nickname = string.Empty;

    public int missedHeartbeats;

    public Client(TcpClient client)
    {
        this.client = client;
        stream = client.GetStream();

        _ = HeartbeatLoop();
    }

    async Task HeartbeatLoop()
    {
        while (!disconnected)
        {
            await Task.Delay(4000); // 3 sec on client, 4 here.

            missedHeartbeats++;

            if (missedHeartbeats < 5) continue;

            Disconnect(DisconnectCode.HeartbeatTimeout);
        }
    }

    public async Task Send(byte[] data)
    {
        EncrpytionHelper.Encrypt(data);

        await stream.WriteAsync(data);
    }

    public void Disconnect(DisconnectCode code)
    {
        if (disconnected) return;

        client.Close();
        client.Dispose();

        disconnected = true;

        Logger.Info($"Client '{id}' disconnected, reason: '{code}'");
    }
}
