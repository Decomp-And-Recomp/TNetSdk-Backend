using System.Text;

namespace TNet.Server.Data;

internal struct SerializedRoomInfo
{
    public ushort roomId {  get; private set; }
    public ushort groupId { get; private set; }
    public ushort masterId { get; private set; } // master client id
    public ushort onlineUsers { get; private set; }
    public ushort maxUsers { get; private set; }
    public ushort state { get; private set; } //TNetRoom.isGaming (Client)
    public ushort passworded { get; private set; }                                                                      
    public byte[] roomName { get; private set; }
    public byte[] creatorName { get; private set; }
    public byte[] roomComment { get; private set; }

    public static SerializedRoomInfo FromRoom(Room room)
    {
        if (room.owner == null)
        {
            Debug.LogError("Cannot create SerializedRoomInfo without owner being set, this CANNOT happen and requires attention.");
            return new();
        }

        byte[] ownerNameArray = new byte[16], roomNameArray = new byte[16], roomCommentArray = new byte[64];

        byte[] tempArray;

        tempArray = Encoding.ASCII.GetBytes(room.owner.nickname ?? "");
        for (int i = 0; i < ownerNameArray.Length; i++)
            ownerNameArray[i] = i < tempArray.Length ? tempArray[i] : (byte)0;

        tempArray = Encoding.ASCII.GetBytes(room.name ?? "");
        for (int i = 0; i < roomNameArray.Length; i++)
            roomNameArray[i] = i < tempArray.Length ? tempArray[i] : (byte)0;

        tempArray = Encoding.ASCII.GetBytes(room.comment ?? "");
        for (int i = 0; i < roomCommentArray.Length; i++)
            roomCommentArray[i] = i < tempArray.Length ? tempArray[i] : (byte)0;

        return new()
        {
            roomId = room.id,
            groupId = room.groupId,
            masterId = room.owner.id,
            onlineUsers = (ushort)room.clients.Count,
            maxUsers = room.maxUsers,
            state = room.state == Room.State.started ? (ushort)1 : (ushort)0,
            passworded = string.IsNullOrWhiteSpace(room.password) ? (ushort)0 : (ushort)1,
            creatorName = ownerNameArray,
            roomName = roomNameArray,
            roomComment = roomCommentArray
        };
    }

}
