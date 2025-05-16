using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Notifications;

internal static class RoomLeaveNotifyCmd
{
    /// <summary> Used in <see cref="Room.RemoveClient(Client)"/>.</summary>
    /// <param name="userId">Id of leaving user.</param>
    /// <returns>Packet that has to be sent to each player in the room.</returns>
    public static Packet Notify(ushort userId)
    {
        Packer packer = new();

        packer.PushUInt16(userId);

        return packer.MakePacket(RoomCMD.leave_notify);
    }
}
