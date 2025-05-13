using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Data;

namespace TNet.Server.Cmd;

internal static class RoomJoinResCmd
{
	public static Packet Response(RoomJoinResult result, ushort sitPos, SerializedRoomInfo roomInfo)
    {
        Packer packer = new();

        packer.PushUInt16((ushort)result);
        packer.PushUInt16(sitPos);

        packer.PushUInt16(roomInfo.roomId);
        packer.PushUInt16(roomInfo.groupId);
        packer.PushUInt16(roomInfo.masterId);
        packer.PushUInt16(roomInfo.onlineUsers);
        packer.PushUInt16(roomInfo.maxUsers);
        packer.PushUInt16(roomInfo.state);
        packer.PushUInt16(roomInfo.passworded);

        packer.PushByteArray(roomInfo.creatorName, 16);
        packer.PushByteArray(roomInfo.roomName, 16);
        packer.PushByteArray(roomInfo.roomComment, 64);

		return packer.MakePacket(RoomCMD.join_res);
	}

	public static Packet Response(RoomJoinResult result)
	{
		Packer packer = new();

        packer.PushUInt16((ushort)result);

		return packer.MakePacket(RoomCMD.join_res);
	}
}
