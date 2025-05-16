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
    public static async Task OnSystemHeartbeat(Client client)
    {
        //ushort ping = 0;
        //unPacker.PopUInt16(ref ping); // player is sending the ping, but lets ignore it

        Debug.Log("HEARTBEAT FROM: " + client.id);
        client.missedHeartbeatCounter = 0;

        await LobbyUtils.SendToClient(SysHeartbeatResCmd.Response(0), client);
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

        await LobbyUtils.SendToClient(SysLoginResCmd.Response(LoginResult.ok, client.id, request.nickname), client);
    }
    #endregion

    public static void OnRoomDragList(UnPacker unPacker, Client client)
    {
        if (!RoomDragListCmd.TryParse(unPacker, out var roomDragList))
        {
            LobbyUtils.LogBadUnpacker("OnRoomDragList");
            return;
        }

        _ = LobbyUtils.SendToClient(RoomDragListResCmd.Response(roomDragList.page, roomDragList.pageSplit, roomDragList.listType), client);
    }

    public static void OnRoomCreate(UnPacker unPacker, Client client)
    {
        if (!RoomCreateCmd.TryParse(unPacker, out var cmd))
        {
            LobbyUtils.LogBadUnpacker("OnRoomCreate");
            return;
        }

        if (!Room.TryCreate(cmd, out _, client))
        {
            LobbyUtils.Log("Couldnt create new room. (full?)", ConsoleColor.Red);
            _ = LobbyUtils.SendToClient(RoomCreateResCmd.Response(RoomCreateResCmd.Result.full, 0), client);
        }
    }

    public static void OnRoomJoin(UnPacker unPacker, Client client)
    {
        if (!RoomJoinCmd.TryParse(unPacker, out var cmd))
        {
            LobbyUtils.LogBadUnpacker("OnRoomJoin");
            return;
        }

        if (!Lobby.rooms.TryGetValue(cmd.roomId, out var room))
        {
            _ = LobbyUtils.SendToClient(RoomJoinResCmd.Response(RoomJoinResult.no_exist), client);
            return;
        }

        if (room.state == Room.State.started)
        {
            _ = LobbyUtils.SendToClient(RoomJoinResCmd.Response(RoomJoinResult.gaming), client);
            return;
        }

        if (room.isFull)
        {
            _ = LobbyUtils.SendToClient(RoomJoinResCmd.Response(RoomJoinResult.full), client);
            return;
        }

        _ = room.TryConnectClient(client, cmd.password);
    }

    public static void OnRoomLeave(Client client)
    {
        if (client.room == null)
        {
            Lobby.DisconnectClient(client, DisconnectCode.SuspiciousRequests);
            return;
        }

        client.RemoveFromRoom();
    }

    public static void OnRoomSetVar(UnPacker unPacker, Client client)
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

        client.room.SetRoomVariable(client.id, cmd.key, cmd.var);
    }

    public static void OnRoomSetUserVar(UnPacker unPacker, Client client)
    {
        if (!RoomSetUserVarCmd.TryParse(unPacker, out var cmd))
        {
            LobbyUtils.LogBadUnpacker("OnRoomSetUserVar");
            return;
        }

        if (cmd.data == null)
        {
            Lobby.DisconnectClient(client, DisconnectCode.SuspiciousRequests);
            return;
        }

        client.SetUserVar(cmd.key, cmd.data);
    }

    public static void OnRoomBroadcastMsg(UnPacker unPacker, Client client)
    {
        if (!RoomBroadcastMsgCmd.TryParse(unPacker, out var cmd))
        {
            LobbyUtils.LogBadUnpacker("OnRoomBroadcastMsg");
            Debug.Log(BitConverter.ToString(unPacker.ByteArray()).Replace("-", ""));
            return;
        }

        if (client.room == null || cmd.bytes == null)
        {
            Lobby.DisconnectClient(client, DisconnectCode.SuspiciousRequests);
            return;
        }

        client.room.SendToAll(RoomMsgNotifyCmd.Notify(client.id, cmd.bytes));
    }

    public static void OnRoomLockReq(UnPacker unPacker, Client client)
    {
        if (!RoomLockReqCmd.TryParse(unPacker, out var cmd))
        {
            LobbyUtils.LogBadUnpacker("OnRoomLockReq");
            return;
        }

        if (client.room == null)
        {
            Lobby.DisconnectClient(client, DisconnectCode.SuspiciousRequests);
            return;
        }

        client.room.Lock(client, cmd.password);

        //client.room.SendToAll(RoomMsgNotifyCmd.Notify(client.id, cmd.bytes));
    }

    public static void OnRoomStart(Client client)
    {
        if (client.room == null)
        {
            Lobby.DisconnectClient(client, DisconnectCode.SuspiciousRequests);
            return;
        }

        client.room.Start(client);

        //client.room.SendToAll(RoomMsgNotifyCmd.Notify(client.id, cmd.bytes));
    }
}
