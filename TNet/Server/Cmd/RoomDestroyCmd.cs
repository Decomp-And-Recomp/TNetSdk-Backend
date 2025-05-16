using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomDestroyCmd : RoomCmd
{
	public Packet MakePacket()
	{
		return MakePacket(SysCMD.logout);
	}
}
