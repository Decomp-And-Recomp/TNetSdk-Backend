using TNet.Server.Binary;
using TNet.Server.Binary.Protocol;
using TNet.Server.Cmd;
using TNet.Server.Data;
using TNet.Server.Notifications;
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

        await SendToClient(SysHeartbeatResCmd.Response(0), client);
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

        await SendToClient(SysLoginResCmd.Response(LoginResult.ok, client.id, request.nickname), client);
    }
    #endregion

    public static async Task OnRoomDragList(UnPacker unPacker, Client client)
    {
        if (!RoomDragListCmd.TryParse(unPacker, out var roomDragList))
        {
            LobbyUtils.LogBadUnpacker("OnRoomDragList");
            return;
        }

        await SendToClient(RoomDragListResCmd.Response(roomDragList.page, roomDragList.pageSplit, roomDragList.listType), client);
    }

    public static async Task OnRoomCreate(UnPacker unPacker, Client client)
    {
        if (!RoomCreateCmd.TryParse(unPacker, out var cmd))
        {
            LobbyUtils.LogBadUnpacker("OnRoomCreate");
            return;
        }

        if (!Room.TryCreate(cmd, out var room, client))
        {
            LobbyUtils.Log("Couldnt create new room. (full?)", ConsoleColor.Red);
            await SendToClient(RoomCreateResCmd.Response(RoomCreateResCmd.Result.full, 0), client);
            return;
        }

        await SendToClient(RoomCreateResCmd.Response(RoomCreateResCmd.Result.ok, room.id), client);

        await SendToClient(RoomJoinNotifyCmd.Notify(client), client);
    }

    public static void OnRoomLeave(Client client)
    {
        if (client.room == null)
        {
            Lobby.DisconnectClient(client, DisconnectCode.SuspiciousRequests);
            return;
        }

        _ = client.RemoveFromRoom();
    }

    public static async Task OnRoomSetVar(UnPacker unPacker, Client client)
    {
        if (!RoomSetVarCmd.TryParse(unPacker, out var cmd))
        {
            LobbyUtils.LogBadUnpacker("OnRoomSetVar");
            return;
        }

        if (cmd.var == null || client.room == null)
        {
            Lobby.DisconnectClient(client, DisconnectCode.SuspiciousRequests);
            return;
        }

        foreach (Client c in client.room.clients)
            _ = SendToClient(RoomVarNotifyCmd.Response(client.id, cmd.key, cmd.var), c);
    }

    public static async Task SendToClient(Packet packet, Client client)
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
