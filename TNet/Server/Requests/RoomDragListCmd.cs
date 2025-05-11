using TNet.Server.Binary.Protocol;
using TNet.Server.Data;

namespace TNet.Server.Requests;

internal class RoomDragListCmd
{
    public ushort groupId;
    public ushort page;
    public ushort pageSplit;
    public RoomDragListType listType;

    RoomDragListCmd() { }

    public static bool TryParse(UnPacker unPacker, out RoomDragListCmd result)
    {
        result = new();

        if (!unPacker.PopUInt16(ref result.groupId)) return false;
        if (!unPacker.PopUInt16(ref result.page)) return false;
        if (!unPacker.PopUInt16(ref result.pageSplit)) return false;

        ushort temp = 0;
        if (!unPacker.PopUInt16(ref temp)) return false;

        result.listType = (RoomDragListType)temp;

        return true;
    }
}
