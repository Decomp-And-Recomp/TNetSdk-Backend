using TNet.Server.Binary.Protocol;
using TNet.Server.Binary;

namespace TNet.Server.Notifications;

internal static class RoomStartNotifyCmd
{
    public static Packet Notify(ushort userId)
    {
        Packer packer = new();
        packer.PushUInt16(userId);

        return packer.MakePacket(RoomCMD.start_notify);
    }
}
