using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Cmd;

namespace TNet.Server.Requests;

internal class RoomSetUserVarCmd : RoomCmd
{
    public ushort key;
    public byte[]? data;

    RoomSetUserVarCmd() { }

    public static bool TryParse(UnPacker unPacker, out RoomSetUserVarCmd result)
    {
        result = new();

        if (!unPacker.PopUInt16(ref result.key)) return false;

        ushort length = 0;

        if (!unPacker.PopUInt16(ref length)) return false;

        result.data = new byte[length];

        if (!unPacker.PopByteArray(ref result.data, length)) return false;

        return true;
    }
}
