using System.Text;
using TNet.Exceptions;

namespace TNet.Server.Data;

internal class SerializedRoomInfo
{
    public ushort roomId {  get; private set; }
    public ushort groupId { get; private set; }
    public ushort masterId { get; private set; } // master client id
    public ushort onlineUsers { get; private set; }
    public ushort maxUsers { get; private set; }
    public ushort state { get; private set; } //TNetRoom.isGaming (Client)
    public ushort passworded { get; private set; }
    public byte[] roomName { get; private set; } = null!;
    public byte[] creatorName { get; private set; } = null!;
    public byte[] roomComment { get; private set; } = null!;

    SerializedRoomInfo() { }

    public static SerializedRoomInfo FromRoom(Room room)
    {
        if (room.owner == null)
            throw new MissingRoomOwnerException();

        byte[] ownerNameArray = new byte[16], roomNameArray = new byte[16], roomCommentArray = new byte[64];

        byte[] tempArray;

        string tempString = room.owner.nickname;
        while (tempString.Length < 16) tempString += "\0";

        ownerNameArray = Encoding.ASCII.GetBytes(tempString);

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
