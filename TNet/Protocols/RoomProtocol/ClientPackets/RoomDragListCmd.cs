using TNet.Protocols.Objects;

namespace TNet.Protocols.RoomProtocol.ClientPackets;

internal class RoomDragListCmd : IClientPacket
{
    public ushort groupId;
    public ushort page;
    public ushort pageSplit;
    public RoomDragListType listType;

    public bool Parse(UnPacker unPacker)
    {
        if (!unPacker.PopUInt16(ref groupId)) return false;
        if (!unPacker.PopUInt16(ref page)) return false;
        if (!unPacker.PopUInt16(ref pageSplit)) return false;

        ushort t = 0;
        if (!unPacker.PopUInt16(ref t)) return false;
        listType = (RoomDragListType)t;

        return true;
    }
}
