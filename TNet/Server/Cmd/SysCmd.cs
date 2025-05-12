using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class SysCmd : Packer
{
	public Packet MakePacket(RoomCMD cmd)
	{
		return MakePacket(1, (ushort)cmd);
	}
}
