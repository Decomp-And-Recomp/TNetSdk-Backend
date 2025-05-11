namespace TNet.Server.Data;

internal readonly struct SerializedRoomInfo
{
    /*internal class RoomInfo
    {
        public ushort m_room_id;

        public ushort m_group_id;

        public ushort m_master_id;

        public ushort m_online_user;

        public ushort m_max_user;

        public ushort m_state;

        public ushort m_passworded;

        public string m_room_name;

        public string m_creater_name;

        public string m_comment;
    }*/

    public readonly ushort roomId;
    public readonly ushort groupId;
    public readonly ushort masterId;
    public readonly ushort onlineUsers;
    public readonly ushort maxUsers;
    public readonly ushort state; //TNetRoom.isGaming (Client)
    public readonly ushort passworded; //TNetRoom.isGaming (Client)
    public readonly byte[] roomName;
    public readonly byte[] roomCreatorName;
    public readonly byte[] roomComment;

    public static SerializedRoomInfo FromRoom(Room room)
    {
        return new();
    }
}
