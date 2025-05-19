using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Cmd;

namespace TNet.Server.Requests;

internal class RoomJoinCmd : RoomCmd
{
    public ushort roomId;
    public string password = string.Empty;

    RoomJoinCmd() { }

    public static bool TryParse(UnPacker unPacker, out RoomJoinCmd result)
    {
        result = new();

        if (!unPacker.PopUInt16(ref result.roomId)) return false;
        if (!unPacker.PopString(ref result.password, Encoding.ASCII)) return false;

        return true;
    }
}
