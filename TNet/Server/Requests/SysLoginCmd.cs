using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Requests;

internal class SysLoginCmd
{
    public string account = string.Empty;
    public string password = string.Empty;
    public string nickname = string.Empty;

    SysLoginCmd() { }

    public static bool TryParse(UnPacker unPacker, out SysLoginCmd result)
    {
        result = new();

        if (!unPacker.PopString(ref result.account, Encoding.ASCII)) return false;
        if (!unPacker.PopString(ref result.password, Encoding.ASCII)) return false;
        if (!unPacker.PopString(ref result.nickname, Encoding.ASCII)) return false;

        return true;
    }
}
