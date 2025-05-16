using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Data;

namespace TNet.Server.Responses;

internal static class SysLoginResCmd
{
    /// <summary> A person has to be logged into the system first, registering his nickname. </summary>
    /// <param name="result">The result.</param>
    /// <param name="userId">A unique user id (session id?)</param>
    /// <param name="nickname">If the nickname contains.. uh.. just count it as "moderated" nickname.</param>
    /// <returns>A packet response that has to be sent to the request's client.</returns>
    public static Packet Response(LoginResult result, ushort userId, string nickname)
    {
        Packer resultPacket = new();
        resultPacket.PushUInt16((ushort)result);
        resultPacket.PushUInt16(userId);
        resultPacket.PushString(nickname, System.Text.Encoding.ASCII);

        return resultPacket.MakePacket(SysCMD.login_res);
    }
}
