using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Responses;

internal static class SysHeartbeatResCmd
{
    public static Packet Response(ulong serverTime)
    {
        Packer result = new();
        result.PushUInt64(serverTime);

        return result.MakePacket(Protocol.sys, CMD.sys_heartbeat_res);
    }
}
