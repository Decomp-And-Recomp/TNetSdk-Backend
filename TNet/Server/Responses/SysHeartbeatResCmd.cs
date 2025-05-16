using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Responses;

internal static class SysHeartbeatResCmd
{
    /// <summary>
    /// I response to the <see cref="RoomCMD.sys_heartbeat"/>
    /// </summary>
    /// <param name="serverTime">Just keep it as 0, i have no idea.</param>
    /// <returns>Packet that has to be sent as the response to the client.</returns>
    public static Packet Response(ulong serverTime)
    {
        Packer result = new();
        result.PushUInt64(serverTime);

        return result.MakePacket(SysCMD.heartbeat_res);
    }
}
