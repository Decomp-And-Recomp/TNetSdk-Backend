using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Notifications;

internal static class RoomUserVarNotifyCmd
{
    public static Packet Notify(ushort userId, ushort key, byte[] data)
    {
        Packer packer = new();
        packer.PushUInt16(userId);
        packer.PushUInt16(key);

        packer.PushUInt16((ushort)data.Length);
        packer.PushByteArray(data, data.Length);

        return packer.MakePacket(RoomCMD.user_var_notify);
    }
}
