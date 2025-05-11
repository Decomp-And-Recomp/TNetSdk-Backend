using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Requests;

internal class RoomSetVarCmd
{
    public ushort key;
    public byte[]? var;

    public static bool TryParse(UnPacker unPacker, out RoomSetVarCmd cmd)
    {
        cmd = new();

        if (!unPacker.PopUInt16(ref cmd.key)) return false;

        ushort length = 0;
        unPacker.PopUInt16(ref length);

        cmd.var = new byte[length];

        unPacker.PopByteArray(ref cmd.var, length);

        return true;
    }
}
