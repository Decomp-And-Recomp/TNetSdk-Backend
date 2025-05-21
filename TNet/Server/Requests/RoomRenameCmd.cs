using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Requests;

internal class RoomRenameCmd
{
    public string roomName = string.Empty;

    RoomRenameCmd() { }

    public static bool TryParse(UnPacker unPacker, out RoomRenameCmd result)
    {
        result = new();

        return unPacker.PopString(ref result.roomName, Encoding.ASCII);
    }
}
