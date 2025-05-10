using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomLeaveCmd : RoomCmd
{
	public Packet MakePacket()
	{
		return MakePacket(CMD.room_leave);
	}
}
