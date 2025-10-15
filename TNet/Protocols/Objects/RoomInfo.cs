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

    public void FromRoom()
    {

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
        w.PushUInt16(hasPassword ? (ushort)1 : (ushort)0);

        w.PushString(roomName, 16);
        w.PushString(creatorName, 16);
        w.PushString(comment, 64);
    }

    public bool FromReader(BufferReader w)
    {
        return false;
    }
}
