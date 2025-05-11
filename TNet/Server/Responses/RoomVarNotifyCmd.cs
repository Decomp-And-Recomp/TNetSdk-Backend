using System.Net.Sockets;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Responses;

internal static class RoomVarNotifyCmd
{
    public static Packet Response(ushort userId, ushort key, byte[] data)
    {
        Packer packer = new();
        packer.PushUInt16(userId);
        packer.PushUInt16(key);

        packer.PushUInt16((ushort)data.Length);
        packer.PushByteArray(data, data.Length);

        return packer.MakePacket(Protocol.room, CMD.room_var_notify);
    }
}
