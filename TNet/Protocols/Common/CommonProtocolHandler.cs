using TNet.Protocols.Common.Client;
using TNet.Protocols.Common.Server;
using XClient = TNet.Client;

namespace TNet.Protocols.Common;

internal class CommonProtocolHandler : ProtocolHandler
{
    public override void Handle(XClient client, UnPacker unPacker)
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
                Logger.Error($"Cannot handle Common cmd '{(Cmd)unPacker.cmd}'");
                return;
        }
    }

    static void OnHeartbeat(XClient client, UnPacker unPacker)
    {
        HeartbeatCmd cmd = new();

        if (!cmd.Parse(unPacker))
        {
            Logger.Error("Unable to parse 'HeartbeatCmd'.");
            return;
        }

        HeartbeatCmdRes response = new()
        {
            m_server_time = 0
        };

        client.missedHeartbeats = 0;

        _ = client.Send(response.Pack());
    }

    static void OnLogin(XClient client, UnPacker unPacker)
    {
        LoginCmd cmd = new();

        if (!cmd.Parse(unPacker))
        {
            Logger.Error("Unable to parse 'LoginCmd'.");
            return;
        }

        LoginCmdRes response = new()
        {
            result = LoginCmdRes.Result.Ok,
            userId = client.id,
            nickname = cmd.nickname
        };

        client.nickname = cmd.nickname;

        Logger.Info($"Client '{client.id}' idenefied as '{client.nickname}'.");

        _ = client.Send(response.Pack());

    }
}
