using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomSendMsgCmd : RoomCmd // UNUSED ON CLIENT, RoomBroadcastMsgCmd.cs used instead.
{
	public RoomSendMsgCmd(ushort user_id, byte[] msg_bytes)
	{
		PushUInt16(user_id);
		PushUInt16((ushort)msg_bytes.Length);
		PushByteArray(msg_bytes, msg_bytes.Length);
	}

	public Packet MakePacket()
	{
		return MakePacket(RoomCMD.send_msg);
	}
}
