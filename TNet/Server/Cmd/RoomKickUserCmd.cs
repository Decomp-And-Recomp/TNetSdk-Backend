using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomKickUserCmd : RoomCmd
{
	public RoomKickUserCmd(ushort user_id)
	{
		PushUInt16(user_id);
	}

	public Packet MakePacket()
	{
		return MakePacket(RoomCMD.kick_user);
	}
}
