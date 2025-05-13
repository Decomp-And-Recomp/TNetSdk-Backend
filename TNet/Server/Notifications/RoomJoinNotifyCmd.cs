using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Notifications;

internal static class RoomJoinNotifyCmd
{
    /// <summary>Creates an notification for all people, that this person joined in the room.
    /// Uses <see cref="Client.room"/> for the room some meta.</summary>
    public static Packet Notify(Client client)
    {
        if (client.room == null)
        {
            Debug.LogError("Client does not have a room assigned.");
            throw new Exception("Client does not have a room assigned.");
        }

        Packer packer = new();

        packer.PushUInt16(client.id);
        packer.PushString(client.nickname, System.Text.Encoding.ASCII);

        packer.PushUInt16((ushort)client.room.clients.IndexOf(client));

        return packer.MakePacket(RoomCMD.join_notify);
    }
}
