using TNet.Protocols.Objects;
using TNet.Protocols.RoomProtocol.ClientPackets;
using TNet.Protocols.RoomProtocol.ServerPackets;

namespace TNet.Protocols.RoomProtocol;

internal class RoomProtocolHandler : ProtocolHandler
{
    public override void Handle(Client client, UnPacker unPacker)
    {
        if (!client.loggedIn) return;

        switch ((Cmd)unPacker.cmd)
        {
            case Cmd.Create:
                OnCreate(client, unPacker);
                return;
            case Cmd.Join:
                OnJoin(client, unPacker);
                return;
            case Cmd.Rename:
                OnRename(client, unPacker);
                return;
            case Cmd.SetUserVar:
                OnSetUserVar(client, unPacker);
                return;
            case Cmd.SetVar:
                OnSetVar(client, unPacker);
                return;
            case Cmd.Leave:
                OnLeave(client, unPacker);
                return;
            case Cmd.Start:
                OnStart(client, unPacker);
                return;
            case Cmd.BroadcastMessage:
                OnBroadcastMessage(client, unPacker);
                return;
            case Cmd.SendMessage:
                OnSendMessage(client, unPacker);
                return;
            case Cmd.LockRequest:
                OnLock(client, unPacker);
                return;
            case Cmd.UnlockRequest:
                OnUnlock(client, unPacker);
                return;
            case Cmd.DragList:
                OnDragList(client, unPacker);
                return;
            case Cmd.Destroy:
                OnDestroy(client, unPacker);
                return;
            default:
                throw new Exception($"Cannot handle Room cmd '{(Cmd)unPacker.cmd}'");
        }
    }

    static void OnCreate(Client client, UnPacker unPacker)
    {
        if (client.room != null) return;

        RoomCreateCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomCreateCmd'.");

        RoomCreateResCmd response = new();

        var room = Room.TryCreate(cmd, client);

        if (room == null)
        {
            _ = client.Send(response.Pack());
            return;
        }

        response.success = true;
        response.roomId = room.id;
        _ = client.Send(response.Pack());

        if (Variables.version != Version.Gen1) return;

        RoomJoinResCmd joinResponse = new()
        {
            result = Objects.RoomJoinResult.Ok
        };

        joinResponse.roomInfo.FromRoom(room);

        _ = client.Send(joinResponse.Pack());
    }

    static void OnJoin(Client client, UnPacker unPacker)
    {
        if (client.room != null) return;

        RoomJoinCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomJoinCmd'.");

        if (!Lobby.rooms.TryGetValue(cmd.id, out var room))
        {
            RoomJoinResCmd result = new()
            {
                result = Objects.RoomJoinResult.DoesntExist
            };
            _ = client.Send(result.Pack());
            return;
        }

        room.TryAdd(cmd, client);
    }

    static void OnRename(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        RoomRenameCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomRenameCmd'.");

        client.room.Rename(cmd, client);
    }

    static void OnSetUserVar(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        RoomSetUserVarCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'SetUserVarCmd'.");

        client.room.SetUserVar(cmd, client);
    }

    static void OnSetVar(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        RoomSetVarCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomSetVarCmd'.");

        client.room.SetVar(cmd, client);
    }

    static void OnLeave(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        client.room.Disconnect(client);
    }

    static void OnStart(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        client.room.Start(client);
    }

    static void OnBroadcastMessage(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        RoomBroadcastMsgCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomBroadcastMsgCmd'.");

        client.room.Broadcast(client, cmd.data);
    }

    static void OnSendMessage(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        RoomSendMsgCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomSendMsgCmd'.");

        client.room.SendToId(cmd.userId, client, cmd.data);
    }

    static void OnLock(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        RoomLockReqCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomLockReqCmd'.");

        client.room.Lock(client, cmd.key);
    }

    static void OnUnlock(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        RoomUnlockReqCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomUnlockReqCmd'.");

        client.room.Unlock(client, cmd.key);
    }

    static void OnDragList(Client client, UnPacker unPacker)
    {
        if (client.room != null) return;

        RoomDragListCmd cmd = new();

        if (!cmd.Parse(unPacker)) throw new Exception("Unable to parse 'RoomDragListCmd'.");

        var rooms = cmd.listType switch
        {
            RoomDragListType.All => Lobby.rooms
            .OrderBy(kv => kv.Key)
            .Skip((cmd.page - 1) * cmd.pageSplit)
            .Take(cmd.pageSplit)
            .Select(kv => kv.Value),

            RoomDragListType.NotFull => Lobby.rooms
            .Where(kv => kv.Value.GetOnline() < kv.Value.maxClients)
            .OrderBy(kv => kv.Key)
            .Skip((cmd.page - 1) * cmd.pageSplit)
            .Take(cmd.pageSplit)
            .Select(kv => kv.Value),

            RoomDragListType.NotFullNotStarted => Lobby.rooms
            .Where(kv => !kv.Value.started)
            .Where(kv => kv.Value.GetOnline() < kv.Value.maxClients)
            .OrderBy(kv => kv.Key)
            .Skip((cmd.page - 1) * cmd.pageSplit)
            .Take(cmd.pageSplit)
            .Select(kv => kv.Value),

            _ => throw new Exception("Unacceptable list type OnDragList")
        };

        RoomDragListResCmd res = new()
        {
            page = cmd.page,
            pageSum = cmd.pageSplit,
            listType = cmd.listType,
            rooms = rooms
        };

        _ = client.Send(res.Pack());
    }

    static void OnDestroy(Client client, UnPacker unPacker)
    {
        if (client.room == null) return;

        RoomDestroyResCmd res = new()
        {
            success = client.isRoomOwner
        };

        if (res.success) client.room.Close();

        _ = client.Send(res.Pack());
    }
}
