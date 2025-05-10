using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class RoomSetVarCmd
{
	public readonly ushort key;
	public readonly byte[] var;

	/*public RoomSetVarCmd(ushort key, byte[] msg_bytes)
	{
		PushUInt16(key);
		PushUInt16((ushort)msg_bytes.Length);
		PushByteArray(msg_bytes, msg_bytes.Length);
	}*/

	public RoomSetVarCmd(UnPacker unPacker)
	{
		unPacker.PopUInt16(ref key);

		ushort length = 0;
		unPacker.PopUInt16(ref length);
		var = new byte[length];
		unPacker.PopByteArray(ref var, length);
	}
}
