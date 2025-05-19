using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;

namespace TNet.Server.Requests;

internal class RoomSendMsgCmd
{
    public ushort user_id;
    public byte[] data = null!;

    RoomSendMsgCmd() { }

    public static bool TryParse(UnPacker unPacker, out RoomSendMsgCmd cmd)
    {
        cmd = new();

        if (!unPacker.PopUInt16(ref cmd.user_id)) return false;

        ushort length = 0;
        if (!unPacker.PopUInt16(ref length)) return false;
        cmd.data = new byte[length];

        if (!unPacker.PopByteArray(ref cmd.data, length)) return false;

        return true;
    }
}
