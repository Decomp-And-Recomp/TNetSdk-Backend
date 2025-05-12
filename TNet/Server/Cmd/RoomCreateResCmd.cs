using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.ClientJunk;

namespace TNet.Server.Cmd;

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

        return resultPacket.MakePacket(2, (ushort)RoomCMD.create_res);
    }
}
