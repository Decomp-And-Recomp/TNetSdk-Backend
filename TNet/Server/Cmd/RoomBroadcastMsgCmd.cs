using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomBroadcastMsgCmd : RoomCmd
{
	public RoomBroadcastMsgCmd(byte[] msg_bytes)
	{
		PushUInt16((ushort)msg_bytes.Length);
		PushByteArray(msg_bytes, msg_bytes.Length);
	}

	public Packet MakePacket()
	{
		return MakePacket(RoomCMD.room_broadcast_msg);
	}
}
