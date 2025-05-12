using System.Text;
using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomCommentChangeReqCmd : RoomCmd
{
	public RoomCommentChangeReqCmd(ushort room_id, string parm)
	{
		PushUInt16(room_id);
		byte[] bytes = Encoding.ASCII.GetBytes(parm);
		PushUInt16((ushort)bytes.Length);
		PushByteArray(bytes, bytes.Length);
	}

	public Packet MakePacket()
	{
		return MakePacket(RoomCMD.set_create_param);
	}
}
