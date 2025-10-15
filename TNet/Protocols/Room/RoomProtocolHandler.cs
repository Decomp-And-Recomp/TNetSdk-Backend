using TNet.Protocols.Room.Client;
using TNet.Protocols.Room.Server;

using XClient = TNet.Client;

namespace TNet.Protocols.Room;

internal class RoomProtocolHandler : ProtocolHandler
{
    public override void Handle(XClient client, UnPacker unPacker)
    {
        switch ((Cmd)unPacker.cmd)
        {
            case Cmd.Create:
                OnCreate(client, unPacker);
                return;
            default:
                Logger.Error($"Cannot handle Room cmd '{(Cmd)unPacker.cmd}'");
                return;
        }
    }

    static void OnCreate(XClient client, UnPacker unPacker)
    {
        RoomCreateCmd cmd = new();

        if (!cmd.Parse(unPacker))
        {
            Logger.Error("Unable to parse 'RoomCreateCmd'.");
            return;
        }

        RoomCreateResCmd response = new()
        {
            result = RoomCreateResCmd.Result.Ok,
            roomId = 0
        };

        RoomJoinResCmd temp = new()
        {
            result = RoomJoinResCmd.Result.Ok
        };

        temp.roomInfo.roomId = 0;
        temp.roomInfo.roomName = cmd.roomName;
        temp.roomInfo.creatorName = client.nickname;
        temp.roomInfo.online = 1;
        temp.roomInfo.masterId = client.id;
        temp.roomInfo.maxClients = cmd.maxClients;

        _ = client.Send(response.Pack());
        _ = client.Send(temp.Pack());
    }
}
