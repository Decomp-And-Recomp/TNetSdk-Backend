using TNet.Binary;

namespace TNet.Protocols.Objects;

internal class RoomInfo
{
    public ushort roomId;
    public ushort groupId;
    public ushort masterId;
    public ushort online;
    public ushort maxClients;
    public bool started;
    public bool hasPassword;
    public string roomName = string.Empty;
    public string creatorName = string.Empty;
    public string comment = string.Empty;

    public void FromRoom(Room room)
    {
        roomId = room.id;
        groupId = room.groupId;
        masterId = room.owner.id;
        online = room.GetOnline();
        maxClients = room.maxClients;
        started = room.started;
        hasPassword = !string.IsNullOrEmpty(room.password);
        roomName = room.name;
        creatorName = room.owner.nickname;
        comment = room.comment;
    }

    public void ToWriter(BufferWriter w)
    {
        w.PushUInt16(roomId);
        w.PushUInt16(groupId);
        w.PushUInt16(masterId);
        w.PushUInt16(online);
        w.PushUInt16(maxClients);
        w.PushUInt16(started ? (ushort)1 : (ushort)0);
        w.PushUInt16(hasPassword ? (ushort)1 : (ushort)0);

        w.PushString(roomName, 16);
        w.PushString(creatorName, 16);
        w.PushString(comment, 64);
    }
}
