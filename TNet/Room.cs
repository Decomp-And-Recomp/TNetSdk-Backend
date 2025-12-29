using System.Collections.Concurrent;
using TNet.Helpers;
using TNet.Protocols.Objects;
using TNet.Protocols.RoomProtocol.ClientPackets;
using TNet.Protocols.RoomProtocol.ServerPackets;

namespace TNet;

internal class Room
{
    public ushort id { get; private set; }
    public Client owner = null!;

    readonly List<Client> clients = [];
    public readonly ConcurrentDictionary<ushort, KeyValuePair<ushort, byte[]>> variables = [];
    public bool started { get; private set; }

    public string name { get; private set; } = string.Empty;
    public string password { get; private set; } = string.Empty;
    public string param { get; private set; } = string.Empty;
    public ushort maxClients { get; private set; }
    public ushort groupId { get; private set; }

    public RoomSwitchMasterType switchMasterType { get; private set; }

    public static Room? TryCreate(RoomCreateCmd cmd, Client client)
    {
        if (Lobby.Rooms.Count >= Variables.MaxRooms) return null;

        Room result = new()
        {
            name = cmd.roomName,
            password = cmd.password,
            maxClients = cmd.maxClients,
            param = cmd.param,
            groupId = cmd.groupId,
            owner = client,
            switchMasterType = cmd.switchMasterType
        };

        ushort id;

        while (true)
        {
            id = RandomHelper.GetRoomId();

            if (Lobby.Rooms.ContainsKey(id)) continue;

            result.id = id;

            if (!Lobby.Rooms.TryAdd(id, result)) continue;

            break;
        }

        client.room = result;
        result.clients.Add(client);

        Logger.Info($"Room created, id: '{result.id}', name: {result.name}.");
        return result;
    }

    public ushort GetOnline() => (ushort)clients.Count;

    public void TryAdd(RoomJoinCmd cmd, Client client)
    {
        if (clients.Contains(client))
        {
            Logger.Error("Tried adding a client that is already in this room.");
            // disconnect client here.
            return;
        }

        RoomJoinResCmd result = new();

        if (password != cmd.password) result.result = RoomJoinResult.PasswordMismatch;
        else if (started) result.result = RoomJoinResult.AlreadyStarted;
        else if (clients.Count >= maxClients) result.result = RoomJoinResult.Full;

        if (result.result != RoomJoinResult.Ok)
        {
            _ = client.Send(result.Pack());

            return;
        }

        client.room = this;
        clients.Add(client);

        result.index = (ushort)(clients.Count - 1);
        result.roomInfo.FromRoom(this);

        _ = client.Send(result.Pack());

        foreach (var v in variables)
        {
            RoomUserVarNotifyCmd varNotif = new()
            {
                userId = v.Key,
                key = v.Value.Key,
                data = v.Value.Value
            };

            _ = client.Send(varNotif.Pack());
        }

        RoomJoinNotifyCmd notify = new()
        {
            userId = client.id,
            nickname = client.nickname,
            index = result.index,
        };

        SendToAll(notify.Pack(), client);

        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i] == client) continue;

            Client curClient = clients[i];
            int index = i;

            Task.Run(async () => {
                var notify = new RoomJoinNotifyCmd
                {
                    userId = curClient.id,
                    nickname = curClient.nickname,
                    index = (ushort)index,
                };

                await client.Send(notify.Pack());

                foreach (var v in curClient.userVariables)
                    await client.Send(new RoomUserVarNotifyCmd { userId = curClient.id, key = v.Key, data = v.Value }.Pack());
            });
        }

        Logger.Info($"Client '{client.id}' connected to room '{id}'");
    }

    public void Start(Client startedBy)
    {
        if (started)
        {
            Logger.Error("Cannot start already started room.");
            return;
        }

        if (startedBy != owner)
        {
            Logger.Error("Only owners can start the room.");
            return;
        }

        started = true;

        RoomStartNotifyCmd notify = new()
        {
            userId = startedBy.id
        };

        SendToAll(notify.Pack());
    }

    public void SetUserVar(RoomSetUserVarCmd cmd, Client client)
    {
        client.userVariables[cmd.key] = cmd.data;

        RoomUserVarNotifyCmd notify = new()
        {
            userId = client.id,
            key = cmd.key,
            data = cmd.data
        };

        SendToAll(notify.Pack());
    }

    public void SetVar(RoomSetVarCmd cmd, Client client)
    {
        variables[client.id] = new KeyValuePair<ushort, byte[]>(cmd.key, cmd.data);

        RoomVarNotifyCmd notify = new()
        {
            userId = client.id,
            key = cmd.key,
            data = cmd.data
        };

        SendToAll(notify.Pack());
    }

    public void Rename(RoomRenameCmd cmd, Client client)
    {
        name = cmd.name;
        Logger.Info($"Room '{id}', renamed to: '{cmd.name}'");

        RoomRenameNotifyCmd notify = new()
        {
            userId = client.id,
            name = cmd.name
        };

        SendToAll(notify.Pack());
    }

    public void SendToAll(byte[] data)
    {
        for (int i = 0; i < clients.Count; i++)
        {
            _ = clients[i]?.Send(data);
        }
    }

    public void SendToAll(byte[] data, params Client[] except)
    {
        for (int i = 0; i < clients.Count; i++)
        {
            if (except.Contains(clients[i])) continue;

            _ = clients[i]?.Send(data);
        }
    }

    public void SendToId(ushort id, Client sentBy, byte[] data)
    {
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].id != id) continue;

            RoomMsgNotifyCmd notify = new()
            {
                userId = sentBy.id,
                data = data
            };

            _ = clients[i]?.Send(notify.Pack());
            break;
        }
    }

    public void Broadcast(Client sentBy, byte[] data)
    {
        RoomMsgNotifyCmd notify = new()
        {
            userId = sentBy.id,
            data = data
        };

        SendToAll(notify.Pack(), sentBy);
    }

    public void Lock(Client sentBy, string key)
    {
        RoomLockResCmd res = new()
        {
            success = true,
            key = key
        };

        _ = sentBy.Send(res.Pack());
    }

    public void Unlock(Client sentBy, string key)
    {
        RoomUnlockResCmd res = new()
        {
            success = true,
            key = key
        };

        _ = sentBy.Send(res.Pack());
    }

    public void Disconnect(Client client)
    {
        if (!clients.Contains(client))
        {
            Logger.Error("Tried disconnecting client that is not in this room.");
            return;
        }

        RoomLeaveNotifyCmd notifyLeave = new()
        {
            userId = client.id
        };

        SendToAll(notifyLeave.Pack());
        clients.Remove(client);
        client.room = null;

        if (owner == client)
        {
            if (switchMasterType == RoomSwitchMasterType.None || clients.Count < 1)
            {
                Close();
                return;
            }

            owner = clients[0];

            RoomCreaterChangeNotifyCmd notifyChange = new()
            {
                userId = owner.id
            };

            SendToAll(notifyChange.Pack());
        }
    }

    public void Close()
    {
        RoomDestroyNotifyCmd notify = new();

        SendToAll(notify.Pack());

        if (!Lobby.Rooms.TryRemove(id, out var removed)) Logger.Error($"Unable to remove room '{id}' from dictionary.");
        else if (removed != this) Logger.Error($"When trying to remove room '{id}', some other room got removed.");
    }
}
