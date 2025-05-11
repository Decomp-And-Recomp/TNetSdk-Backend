using System.Runtime.InteropServices;
using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Cmd;
using TNet.Server.Data;
using TNet.Server.Requests;
using TNet.Server.Responses;

namespace TNet.Server;

internal static class LobbyCmdImpl
{
    #region system
    public static async Task OnSystemHeartbeat(UnPacker unPacker, Client client)
    {
        ushort ping = 0;
        unPacker.PopUInt16(ref ping);

        await Send(SysHeartbeatResCmd.Response(0), client);
    }

    public static async Task OnSystemPlayerLogin(UnPacker unPacker, Client client)
    {
        if (!SysLoginCmd.TryParse(unPacker, out var request))
        {
            LobbyUtils.LogBadUnpacker("OnRoomDragList");
            //await Send(SysLoginResCmd.Response(SysLoginResCmd.Result.error_pwd, client.id, request.nickname), client);
            return;
        }

        client.isLogged = true;
        client.nickname = request.nickname;

        LobbyUtils.Log($"New user '{request.account}' idenefied as \"{request.nickname}\"");

        await Send(SysLoginResCmd.Response(LoginResult.ok, client.id, request.nickname), client);
    }
    #endregion

    public static async Task OnRoomDragList(UnPacker unPacker, Client client)
    {
        if (!RoomDragListCmd.TryParse(unPacker, out var roomDragList))
        {
            LobbyUtils.LogBadUnpacker("OnRoomDragList");
            return;
        }

        await Send(RoomDragListResCmd.Response(roomDragList.page, roomDragList.pageSplit, roomDragList.listType), client);
    }

    public static async Task OnRoomCreate(UnPacker unPacker, Client client)
    {
        if (!RoomCreateCmd.TryParse(unPacker, out var cmd))
        {
            LobbyUtils.LogBadUnpacker("OnRoomCreate");
            return;
        }

        if (!Room.TryCreate(cmd, out var room))
        {
            LobbyUtils.Log("Couldnt create new room. (full?)", ConsoleColor.Red);
            await Send(RoomCreateResCmd.Response(RoomCreateResCmd.Result.full, 0), client);
            return;
        }

        await Send(RoomCreateResCmd.Response(RoomCreateResCmd.Result.ok, room.id), client);
    }

    public static async Task OnRoomSetVar(UnPacker unPacker, Client client)
    {
        RoomSetVarCmd cmd = new(unPacker);

        //LobbyUtils.Log(Convert.ToBase64String(cmd.var), ConsoleColor.DarkCyan);
        //await Send(RoomVarNotifyCmd.Response(client.id, cmd.key, cmd.var), client);
    }

    static async Task Send(Packet packet, Client client)
    {
        LobbyUtils.Encrypt(packet, Lobby.blowFish);

        byte[] bytes = new byte[packet.Length];

        packet.Position = 0;

        if (!packet.PopByteArray(ref bytes, packet.Length-1))
        {
            Debug.LogError("Packet Fail");
            return;
        }

        await client.connection.GetStream().WriteAsync(bytes);
    }
}
