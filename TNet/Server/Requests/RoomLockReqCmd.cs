using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Requests;

internal static class RoomLockReqCmd
{
    public static bool TryParse(UnPacker unPacker, out string result)
    {
        result = "";
        return unPacker.PopString(ref result, Encoding.ASCII);
    }
}
