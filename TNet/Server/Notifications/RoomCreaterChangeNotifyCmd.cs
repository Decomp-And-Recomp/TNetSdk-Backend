using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Notifications;

internal class RoomCreaterChangeNotifyCmd
{
    /// <summary>Notification for all players that owner of the room just changed.</summary>
    /// <param name="userId">Id of new owner.</param>
    /// <returns>A package that has to be sent to all room clients.</returns>
    public static Packet Notify(ushort userId)
    {
        Packer packer = new();

        packer.PushUInt16(userId);

        return packer.MakePacket(RoomCMD.creater_notify);
    }
}
