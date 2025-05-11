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
    public readonly ushort masterId; // master client id
    public readonly ushort onlineUsers;
    public readonly ushort maxUsers;
    public readonly ushort state; //TNetRoom.isGaming (Client)
    public readonly ushort passworded;
    public readonly byte[] roomName;
    public readonly byte[] creatorName;
    public readonly byte[] roomComment;

    public static SerializedRoomInfo FromRoom(Room room)
    {
        /*
            roomInfo.m_creater_name = Encoding.ASCII.GetString(ByteArray(), Offset, 16);
            roomInfo.m_creater_name = roomInfo.m_creater_name.Substring(0, roomInfo.m_creater_name.IndexOf('\0'));
            Offset += 16;
            roomInfo.m_room_name = Encoding.ASCII.GetString(ByteArray(), Offset, 16);
            roomInfo.m_room_name = roomInfo.m_room_name.Substring(0, roomInfo.m_room_name.IndexOf('\0'));
            Offset += 16;
            roomInfo.m_comment = Encoding.ASCII.GetString(ByteArray(), Offset, 64);
            roomInfo.m_comment = roomInfo.m_comment.Substring(0, roomInfo.m_comment.IndexOf('\0'));
            Offset += 64;
         */
        return new();
    }
}
