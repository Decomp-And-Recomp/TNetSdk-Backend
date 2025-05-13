using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Requests;

internal class RoomSetVarCmd
{
    public ushort key;
    public byte[]? var;

    RoomSetVarCmd() { }

    public static bool TryParse(UnPacker unPacker, out RoomSetVarCmd cmd)
    {
        cmd = new();

        if (!unPacker.PopUInt16(ref cmd.key)) return false;

        ushort length = 0;
        if (!unPacker.PopUInt16(ref length)) return false;

        cmd.var = new byte[length];

        if (!unPacker.PopByteArray(ref cmd.var, length)) return false;

        return true;
    }
}
