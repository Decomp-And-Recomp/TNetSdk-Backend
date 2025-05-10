using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal static class SysHeartbeatResCmd
{
	/*public long m_server_time;

	public override bool MakePacket(Packet packet)
	{
		if (!base.ParserPacket(packet))
		{
			return false;
		}
		ulong val = 0uL;
		if (!PopUInt64(ref val))
		{
			return false;
		}
		m_server_time = (long)val;
		return true;
	}*/

	public static Packet Response(ulong serverTime)
	{
        Packer result = new();
        result.PushUInt64(serverTime);

        return result.MakePacket(Protocol.sys, CMD.sys_heartbeat_res);
	}
}
