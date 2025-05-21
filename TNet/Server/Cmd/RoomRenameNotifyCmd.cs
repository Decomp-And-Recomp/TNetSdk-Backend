using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal static class RoomRenameNotifyCmd
{
	public static Packet Notify(ushort userId, string roomName)
	{
		Packer packer = new();

		packer.PushUInt16(userId);

		packer.PushString(roomName, Encoding.ASCII);

		return packer.MakePacket(RoomCMD.rename_notify);
	}
}
