using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Requests;

internal class RoomLockReqCmd
{
    public string password = string.Empty;

    public static bool TryParse(UnPacker unPacker, out RoomLockReqCmd result)
    {
        result = new();

        if (unPacker.PopString(ref result.password, Encoding.ASCII)) return false;

        return true;
    }
}
