using TNet.Helpers;
using TNet.Protocols.SystemProtocol.ClientPackets;
using TNet.Protocols.SystemProtocol.ServerPackets;

namespace TNet.Protocols.SystemProtocol;

internal class SystemProtocolHandler : ProtocolHandler
{
    public override void Handle(Client client, UnPacker unPacker)
    {
        switch ((Cmd)unPacker.cmd)
        {
            case Cmd.Heartbeat:
                OnHeartbeat(client, unPacker);
                return;
            case Cmd.Login:
                OnLogin(client, unPacker);
                return;
            default:
                throw new Exception($"Cannot handle System cmd '{(Cmd)unPacker.cmd}'");
        }
    }

    static void OnHeartbeat(Client client, UnPacker unPacker)
    {
        HeartbeatCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'HeartbeatCmd'.");

        HeartbeatCmdRes response = new()
        {
            m_server_time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        client.heartValue = 0;

        _ = client.Send(response.Pack());
    }

    static void OnLogin(Client client, UnPacker unPacker)
    {
        LoginCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'LoginCmd'.");

        LoginCmdRes response = new();

        ushort id;

        while (true)
        {
            id = RandomHelper.GetClientId();

            if (Lobby.Clients.ContainsKey(id)) continue;

            if (!Lobby.Clients.TryAdd(id, client)) continue;

            break;
        }

        client.loggedIn = true;
        client.nickname = cmd.nickname;
        response.userId = client.id;
        response.nickname = cmd.nickname;

        Logger.Info($"Client '{client.id}' idenefied as '{client.nickname}'.");

        _ = client.Send(response.Pack());

    }
}
