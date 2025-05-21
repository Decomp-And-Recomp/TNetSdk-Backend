using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Notifications;

internal static class RoomKickUserNotifyCmd
{
    public static Packet Notify(ushort userId)
    {
        Packer p = new();
        p.PushUInt16(userId);

        return p.MakePacket(RoomCMD.kick_user_notify);
    }
}
