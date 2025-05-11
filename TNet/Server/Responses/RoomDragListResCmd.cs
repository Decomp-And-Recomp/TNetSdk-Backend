using System.Text;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Requests;
using TNet.Server.Data;

namespace TNet.Server.Responses;

internal class RoomDragListResCmd
{
    /*

    public ushort m_cur_page;

    public ushort m_page_sum;

    public ushort m_roomlist_type;

    public List<RoomInfo> m_room_info_list;*/

    /*public override bool ParserPacket(Packet packet)
    {
        if (!base.ParserPacket(packet))
        {
            return false;
        }
        if (!PopUInt16(ref m_cur_page))
        {
            return false;
        }
        if (!PopUInt16(ref m_page_sum))
        {
            return false;
        }
        if (!PopUInt16(ref m_roomlist_type))
        {
            return false;
        }
        ushort val = 0;
        if (!PopUInt16(ref val))
        {
            return false;
        }
        m_room_info_list = new List<RoomInfo>();
        for (ushort num = 0; num < val; num++)
        {
            RoomInfo roomInfo = new RoomInfo();
            if (!PopUInt16(ref roomInfo.m_room_id))
            {
                return false;
            }
            if (!PopUInt16(ref roomInfo.m_group_id))
            {
                return false;
            }
            if (!PopUInt16(ref roomInfo.m_master_id))
            {
                return false;
            }
            if (!PopUInt16(ref roomInfo.m_online_user))
            {
                return false;
            }
            if (!PopUInt16(ref roomInfo.m_max_user))
            {
                return false;
            }
            if (!PopUInt16(ref roomInfo.m_state))
            {
                return false;
            }
            if (!PopUInt16(ref roomInfo.m_passworded))
            {
                return false;
            }
            if (!CheckBytesLeft(32))
            {
                return false;
            }
            roomInfo.m_creater_name = Encoding.ASCII.GetString(ByteArray(), Offset, 16);
            roomInfo.m_creater_name = roomInfo.m_creater_name.Substring(0, roomInfo.m_creater_name.IndexOf('\0'));
            Offset += 16;
            roomInfo.m_room_name = Encoding.ASCII.GetString(ByteArray(), Offset, 16);
            roomInfo.m_room_name = roomInfo.m_room_name.Substring(0, roomInfo.m_room_name.IndexOf('\0'));
            Offset += 16;
            roomInfo.m_comment = Encoding.ASCII.GetString(ByteArray(), Offset, 64);
            roomInfo.m_comment = roomInfo.m_comment.Substring(0, roomInfo.m_comment.IndexOf('\0'));
            Offset += 64;
            m_room_info_list.Add(roomInfo);
        }
        return true;
    }*/
}
