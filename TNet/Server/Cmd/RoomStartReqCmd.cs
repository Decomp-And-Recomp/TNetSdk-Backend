using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomStartReqCmd : RoomCmd
{
	public Packet MakePacket()
	{
		return MakePacket(CMD.room_start);
	}
}
