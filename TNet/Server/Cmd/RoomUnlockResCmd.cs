using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class RoomUnlockResCmd : UnPacker
{
	public enum Result
	{
		ok = 0,
		error = 1
	}

	public Result m_result;

	public string m_key = string.Empty;

	public override bool ParserPacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		ushort val = 0;
		if (!PopUInt16(ref val))
		{
			return false;
		}
		m_result = (Result)val;
		ushort val2 = 0;
		if (!PopUInt16(ref val2))
		{
			return false;
		}
		if (!CheckBytesLeft(val2))
		{
			return false;
		}
		m_key = Encoding.ASCII.GetString(ByteArray(), base.Offset, val2);
		base.Offset += val2;
		return true;
	}
}
