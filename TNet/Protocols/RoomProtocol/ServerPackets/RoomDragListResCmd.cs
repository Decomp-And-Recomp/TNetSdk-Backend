using TNet.Protocols.Objects;

namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomDragListResCmd : IServerPacket
{
    public ushort page;
    public ushort pageSum;
    public RoomDragListType listType;
    public IEnumerable<Room>? rooms;

    public byte[] Pack()
    {
        if (rooms == null) throw new Exception("The 'rooms' field cannot be null.");

        Packer p = new();
        p.PushUInt16(page);
        p.PushUInt16(pageSum);
        p.PushUInt16((ushort)listType);

        RoomInfo info = new();
        foreach (var v in rooms)
        {
            info.FromRoom(v);
            info.ToWriter(p);
        }

        return p.MakePacket(Cmd.DragListResponse);
    }
}
