namespace TNet.Server.Cmd;
using TNet.Server.Binary;

internal class RoomSetUserStatusCmd : RoomCmd
{
	public RoomSetUserStatusCmd(ushort key, byte[] msg_bytes)
	{
		PushUInt16(key);
		PushUInt16((ushort)msg_bytes.Length);
		PushByteArray(msg_bytes, msg_bytes.Length);
	}

	public Packet MakePacket()
	{
		return MakePacket(RoomCMD.set_user_status);
	}
}
