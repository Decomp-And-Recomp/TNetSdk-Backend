using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal class RoomDestroyResCmd : UnPacker
{
	public enum Result
	{
		ok = 0
	}

	public Result m_result;

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
		return true;
	}
}
