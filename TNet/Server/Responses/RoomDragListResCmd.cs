using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Data;

namespace TNet.Server.Responses;

internal static class RoomDragListResCmd
{
    /// <summary>Creates a response, takes rooms from <see cref="Lobby.rooms"/>.</summary>
    public static Packet Response(ushort page, ushort pageSum, RoomDragListType listType)
    {
        Packer packer = new();

        packer.PushUInt16(page);
        packer.PushUInt16(pageSum); // page length?
        packer.PushUInt16((ushort)listType);

        List<Room> rooms = listType switch
        {
            RoomDragListType.all => Lobby.rooms
                .OrderBy(kv => kv.Key)
                .Skip((page - 1) * pageSum)
                .Take(pageSum)
                .Select(kv => kv.Value)
                .ToList(),

            RoomDragListType.not_full => Lobby.rooms
                .Where(kv => !kv.Value.isFull)
                .OrderBy(kv => kv.Key)
                .Skip((page - 1) * pageSum)
                .Take(pageSum)
                .Select(kv => kv.Value)
                .ToList(),

            RoomDragListType.not_full_not_game => Lobby.rooms
                .Where(kv => !kv.Value.isFull && kv.Value.state != Room.State.started)
                .OrderBy(kv => kv.Key)
                .Skip((page - 1) * pageSum)
                .Take(pageSum)
                .Select(kv => kv.Value)
                .ToList(),

            _ => [],
        };

        packer.PushUInt16((ushort)rooms.Count);

        SerializedRoomInfo info;

        for (int i = 0; i < rooms.Count; i++)
        {
            info = SerializedRoomInfo.FromRoom(rooms[i]);

            packer.PushUInt16(info.roomId);
            packer.PushUInt16(info.groupId);
            packer.PushUInt16(info.masterId);
            packer.PushUInt16(info.onlineUsers);
            packer.PushUInt16(info.maxUsers);
            packer.PushUInt16(info.state);
            packer.PushUInt16(info.passworded);

            packer.PushByteArray(info.creatorName, 16);
            packer.PushByteArray(info.roomName, 16);
            packer.PushByteArray(info.roomComment, 64);
        }

        return packer.MakePacket(RoomCMD.drag_list_res);
    }
}
