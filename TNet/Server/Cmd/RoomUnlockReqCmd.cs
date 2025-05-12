using System.Text;
using TNet.Server.Binary;

namespace TNet.Server.Cmd;

internal class RoomUnlockReqCmd : RoomCmd
{
	public RoomUnlockReqCmd(string key)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(key);
		PushUInt16((ushort)bytes.Length);
		PushByteArray(bytes, bytes.Length);
	}

	public Packet MakePacket()
	{
		return MakePacket(RoomCMD.unlock_req);
	}
}
