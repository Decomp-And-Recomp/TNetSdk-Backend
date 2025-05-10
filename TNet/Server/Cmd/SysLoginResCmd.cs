using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Cmd;

internal static class SysLoginResCmd
{
	public enum Result
	{
		ok = 0,
		error_pwd = 1
	}

	public static Packet Response(Result result, ushort userId, string nickname)
	{
        Packer resultPacket = new();
        resultPacket.PushUInt16((ushort)result);
        resultPacket.PushUInt16(userId);
		BinaryUtils.PushString(resultPacket, nickname, System.Text.Encoding.ASCII);

        return resultPacket.MakePacket(Protocol.sys, CMD.sys_login_res);
    }

    /*public Result m_result;

	public ushort m_user_id;

	public string m_nickname;

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
		if (!PopUInt16(ref m_user_id))
		{
			return false;
		}
		ushort val2 = 0;
		if (!PopUInt16(ref val2))
		{
			return false;
		}
		if (!CheckBytesLeft(val2))
		{
			return false;
		}
		m_nickname = Encoding.ASCII.GetString(ByteArray(), base.Offset, val2);
		base.Offset += val2;
		return true;
	}

	public override void ToTNetEventData(Packet packet, ref TNetEventData event_data)
	{
		ParserPacket(packet);
		if (m_result == Result.ok)
		{
			//target.Myself = new TNetUser(m_user_id, m_nickname, true);
		}
		event_data.data.Add("result", m_result);
		event_data.data.Add("userId", m_user_id);
		event_data.data.Add("nickName", m_nickname);
	}*/
}
