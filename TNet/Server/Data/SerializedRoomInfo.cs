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
        string InitString(string s, int length)
        {
            while (s.Length < length) s += "\0";
            return s;
        }

        if (room.owner == null)
            throw new MissingRoomOwnerException();

        byte[] nameArray = Encoding.ASCII.GetBytes(InitString(room.owner.nickname, 16));
        byte[] roomNameArray = Encoding.ASCII.GetBytes(InitString(room.owner.nickname, 16));
        byte[] roomCommentArray = Encoding.ASCII.GetBytes(InitString(room.comment, 64));

        nameArray[15] = 0;
        roomNameArray[15] = 0;
        roomCommentArray[63] = 0;

        return new()
        {
            roomId = room.id,
            groupId = room.groupId,
            masterId = room.owner.id,
            onlineUsers = (ushort)room.clients.Count,
            maxUsers = room.maxUsers,
            state = room.state == Room.State.started ? (ushort)1 : (ushort)0,
            passworded = string.IsNullOrWhiteSpace(room.password) ? (ushort)0 : (ushort)1,
            creatorName = nameArray,
            roomName = roomNameArray,
            roomComment = roomCommentArray
        };
    }

}
