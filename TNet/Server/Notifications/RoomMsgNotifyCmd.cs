using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Notifications;

internal static class RoomMsgNotifyCmd
{
    public static Packet Notify(ushort userId, byte[] data)
    {
        Packer packer = new();

        packer.PushUInt16(userId);

        packer.PushUInt16((ushort)data.Length);
        packer.PushByteArray(data, data.Length);

        return packer.MakePacket(RoomCMD.msg_notify);
    }
}
