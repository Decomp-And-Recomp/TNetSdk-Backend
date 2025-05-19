using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class RoomCmd : Packer
{
	public Packet MakePacket(RoomCMD cmd)
	{
		return MakePacket(2, (ushort)cmd);
	}
}
