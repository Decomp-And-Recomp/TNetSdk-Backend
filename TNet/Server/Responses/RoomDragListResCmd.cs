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

        var list = Lobby.rooms.OrderBy(kv => kv.Key) // dont ask me, i took it from chatGPT
            .Skip((page - 1) * pageSum)
            .Take(pageSum)
            .Select(kv => kv.Value)
            .ToList();

        packer.PushUInt16((ushort)list.Count); // sent rooms length

        Debug.LogInfo("Room count: " + list.Count);

        SerializedRoomInfo info;

        for (int i = 0; i < list.Count; i++)
        {
            info = SerializedRoomInfo.FromRoom(list[i]);

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
