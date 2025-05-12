using System.Text;
using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomRenameCmd : RoomCmd
{
	public RoomRenameCmd(string room_name)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(room_name);
		PushUInt16((ushort)bytes.Length);
		PushByteArray(bytes, bytes.Length);
	}

	public Packet MakePacket()
	{
		return MakePacket(RoomCMD.room_rename);
	}
}
