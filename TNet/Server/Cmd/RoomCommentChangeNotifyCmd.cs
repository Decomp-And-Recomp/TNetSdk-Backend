using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class RoomCommentChangeNotifyCmd : UnPacker
{
	public ushort m_user_id;

	public string m_comment = string.Empty;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		if (!PopUInt16(ref m_user_id))
		{
			return false;
		}
		ushort val = 0;
		if (!PopUInt16(ref val))
		{
			return false;
		}
		if (!CheckBytesLeft(val))
		{
			return false;
		}
		m_comment = Encoding.ASCII.GetString(ByteArray(), base.Offset, val);
		base.Offset += val;
		return true;
	}
}
