using TNet.Server.Binary.Protocol;

namespace TNet.Server.Requests;

internal class RoomBroadcastMsgCmd
{
	public byte[] bytes = null!;

	RoomBroadcastMsgCmd() { }

	public static bool TryParse(UnPacker unPacker, out RoomBroadcastMsgCmd result)
	{
		result = new();

		ushort length = 0;
		if (!unPacker.PopUInt16(ref length)) return false;

		result.bytes = new byte[length];

		if (!unPacker.PopByteArray(ref result.bytes, length)) return false;

		return true;
	}
}
