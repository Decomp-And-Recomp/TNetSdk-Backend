using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Responses;

internal static class RoomCreateResCmd
{
    public enum Result
    {
        ok = 0,
        full = 1
    }

    public static Packet Response(Result result, ushort roomId)
    {
        Packer resultPacket = new();
        resultPacket.PushUInt16((ushort)result);
        resultPacket.PushUInt16(roomId);

        return resultPacket.MakePacket(RoomCMD.create_res);
    }
}
