using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Notifications;

internal static class RoomJoinNotifyCmd
{
    public static Packet Notify(Client client)
    {
        if (client.room == null)
            throw new Exception("Client does not have a room assigned.");

        Packer packer = new();

        packer.PushUInt16(client.id);
        packer.PushString(client.nickname, System.Text.Encoding.ASCII);

        packer.PushUInt16((ushort)client.room.clients.IndexOf(client));

        return packer.MakePacket(Protocol.room, CMD.room_join_notify);
    }
}
