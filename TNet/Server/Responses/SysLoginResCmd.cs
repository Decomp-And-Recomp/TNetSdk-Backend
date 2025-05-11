using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Data;

namespace TNet.Server.Responses;

internal static class SysLoginResCmd
{
    public static Packet Response(LoginResult result, ushort userId, string nickname)
    {
        Packer resultPacket = new();
        resultPacket.PushUInt16((ushort)result);
        resultPacket.PushUInt16(userId);
        resultPacket.PushString(nickname, System.Text.Encoding.ASCII);

        return resultPacket.MakePacket(Protocol.sys, CMD.sys_login_res);
    }
}
