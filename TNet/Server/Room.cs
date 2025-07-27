using TNet.Server.Binary;
using TNet.Server.Cmd;
using TNet.Server.Data;
using TNet.Server.Notifications;
using TNet.Server.Requests;
using TNet.Server.Responses;

namespace TNet.Server;

internal class Room : IDisposable
{
    class VariableSet(ushort userId, byte[] data)
    {
        public ushort userId = userId;
        public byte[] data = data;

        public void Set(ushort userId, byte[] data)
        {
            this.userId = userId;
            this.data = data;
        }
    }

    public enum State { open, started, shuttingDown, close }

    public State state { get; private set; } = State.open;

    public ushort id, maxUsers, groupId;

    public string name = string.Empty;
    public string comment = string.Empty;

    public string password = string.Empty;

    public Client? owner;

    public List<Client> clients = [];

    public RoomSwitchMasterType masterSwitchType;

    public RoomType roomType;

    public bool isFull => clients.Count >= maxUsers;

    readonly Dictionary<ushort, VariableSet> vars = [];

    bool isStarting = false;

    Room() { }

    public static bool TryCreate(RoomCreateCmd cmd, out Room room, Client owner)
    {
        room = new();

        bool added = false;

        for (ushort i = 0; i < ushort.MaxValue; i++)
        {
            if (Lobby.rooms.TryAdd(i, room))
            {
                room.id = i;
                added = true;
                break;
            }
        }

        if (!added) return false;

        room.name = cmd.roomName;
        room.comment = cmd.param;
        room.maxUsers = cmd.maxUsers;
        room.groupId = cmd.groupId;
        room.masterSwitchType = cmd.roomSwitchMasterType;
        room.roomType = cmd.roomType;
        room.password = cmd.password;
        room.owner = owner;

        _ = room.CreationLogic();

        Debug.Log($"Created new room with: id={room.id}, maxUsers={room.maxUsers}, {room.masterSwitchType}, {room.roomType}", ConsoleColor.Cyan);

        return true;
    }

    async Task CreationLogic()
    {
        if (owner == null)
        {
            Debug.LogError($"Lobby owner is null, shutting down.");
            ShutDown();
            return;
        }

        try
        {
            await LobbyUtils.SendToClient(RoomCreateResCmd.Response(RoomCreateResCmd.Result.ok, id), owner);

            // TLCK ONLY
            await TryConnectClient(owner, password);
            // else
            //clients.Add(owner);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /*public void SendToId(ushort id, params Packet[] packets)
    {
        foreach (Client c in clients)
        {
            if (c.id == id)
            {
                _ = LobbyUtils.SendToClient(c, packets);
                break;
            }
        }
    }*/
    
    public void SendToId(ushort id, params Packet[] packets)
    {
        if (Lobby.clients.TryGetValue(id, out Client? c))
        {
            if (clients.Contains(c)) _ = LobbyUtils.SendToClient(c, packets);
            else Debug.Log("Player wasnt in the room, id: " + id, ConsoleColor.Red);
        }
        else Debug.Log("PLayer wasnt in Lobby.clients, id: " + id, ConsoleColor.Red);
    }

    public void SendToAll(Packet packet)
    {
        foreach (Client c in clients)
           _ = LobbyUtils.SendToClient(packet, c);
    }

    public async Task SendToAllAsync(Packet packet)
    {
        Task[] tasks = new Task[clients.Count];

        for (int i = 0; i < clients.Count; i++)
        {
            tasks[i] = LobbyUtils.SendToClient(packet, clients[i]);
        }

        await Task.WhenAll(tasks);
    }

    public async Task SendToAllAsync(Packet packet, params Client[] excludeClients)
    {
        List<Task> tasks = [];

        for (int i = 0; i < clients.Count; i++)
        {
            if (excludeClients.Contains(clients[i])) continue;

            tasks.Add(LobbyUtils.SendToClient(packet, clients[i]));
        }

        await Task.WhenAll(tasks);
    }

    public void SendToAll(Packet packet, params Client[] excludeClients)
    {
        foreach (Client c in clients)
           if (!excludeClients.Contains(c)) _ = LobbyUtils.SendToClient(packet, c);
    }

    public async Task TryConnectClient(Client client, string? password)
    {
        if (clients.Contains(client))
        {
            Debug.LogError("Client is already connected to the room.");
            return;
        }

        Debug.LogInfo("Connecting player");

        // Password check is disabled for now
        /*
        if (!string.IsNullOrWhiteSpace(this.password))
        {
            if (string.IsNullOrWhiteSpace(password) || this.password != password)
            {
                await LobbyUtils.SendToClient(RoomJoinResCmd.Response(RoomJoinResult.pwd_error), client);
                return;
            }
        }
        */

        client.room = this;
        clients.Add(client);

        Packet joinRes = RoomJoinResCmd.Response(RoomJoinResult.ok, (ushort)clients.IndexOf(client), SerializedRoomInfo.FromRoom(this));
        await LobbyUtils.SendToClient(joinRes, client);

        await SendToAllAsync(RoomJoinNotifyCmd.Notify(client));

        var notifyTasks = new List<Task>();
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i] == client) continue;
            notifyTasks.Add(LobbyUtils.SendToClient(RoomJoinNotifyCmd.Notify(clients[i]), client));
        }

        foreach (var v in vars)
            notifyTasks.Add(LobbyUtils.SendToClient(RoomVarNotifyCmd.Notify(v.Value.userId, v.Key, v.Value.data), client));

        foreach (var v in clients)
        {
            if (v == client) continue;
            foreach (var set in v.vars)
                notifyTasks.Add(LobbyUtils.SendToClient(RoomUserVarNotifyCmd.Notify(v.id, set.Key, set.Value), client));
        }

        await Task.WhenAll(notifyTasks);

        var syncTasks = new List<Task>();
        foreach (var v in client.vars)
            syncTasks.Add(SendToAllAsync(RoomUserVarNotifyCmd.Notify(client.id, v.Key, v.Value), client));

        await Task.WhenAll(syncTasks);

        Debug.LogInfo("Client connected, count: " + clients.Count);

        if (!isStarting && clients.Count > 1 && state == State.open)
        {
            isStarting = true;

            _ = Task.Run(async () =>
            {
                await Task.Delay(12000);
#nullable disable
                if (state == State.open && clients.Count > 1) Start(owner);
                else isStarting = false;
#nullable enable
            });
        }

        Debug.LogInfo("Connecting player end");
    }

    public void ShutDown()
    {
        state = State.shuttingDown;
        Debug.LogInfo("Shutting a room down");

        if (Lobby.rooms.Remove(id, out var removedRoom))
        {
            // Put it back
            if (removedRoom != this && removedRoom != null)
                Lobby.rooms[id] = removedRoom;
        }

        foreach (Client client in clients) client.RemoveFromRoom();

        state = State.close;
    }

    public void RemoveClient(Client client, bool kick)
    {
        if (state == State.shuttingDown) return;

        if (!clients.Contains(client))
        {
            Debug.LogWarning("Tried disconnectiong a non existend client?");
            return;
        }
        else if (client.room != this)
        {
            Debug.LogWarning("Tried disconnectiong a client that is not in this the room?");
            return;
        }

        // Yes you need to notify the removed player too
        SendToAll(kick
            ? RoomKickUserNotifyCmd.Notify(client.id)
            : RoomLeaveNotifyCmd.Notify(client.id));

        clients.Remove(client);

        client.room = null;

        if (kick) Lobby.DisconnectClient(client, DisconnectCode.RoomKick);
        else if (state == State.started) Lobby.DisconnectClient(client, DisconnectCode.RoomLeave); // yes auto disconnect

        if (owner != client && owner != null) return;

        if (masterSwitchType == RoomSwitchMasterType.Auto && clients.Count > 0)
        {
            if (TryChangeOwner(clients[0])) return;
        }

        Debug.LogInfo("No room owner.");

        ShutDown();
    }

    public void SetRoomVariable(ushort userId, ushort key, byte[] var)
    {
        if (vars.TryGetValue(key, out var v)) v.Set(userId, var);
        else vars[key] = new(userId, var);

        SendToAll(RoomVarNotifyCmd.Notify(userId, key, var));
    }

    public void Start()
    {
        if (state != State.open)
        {
            Debug.LogWarning("Room has to be open, but the state is: " + state);
            return;
        }

        if (owner == null)
        {
            Debug.LogWarning("Cannot start the room without an owner.");
            return;
        }

        state = State.started;

        SendToAll(RoomStartNotifyCmd.Notify(owner.id));
    }

    public void Start(Client startedBy)
    {
        if (state != State.open)
        {
            Debug.LogWarning("Room has to be open, but the state is: " + state);
            return;
        }

        if (!clients.Contains(startedBy))
        {
            Debug.LogWarning("Lobby started by someone not in the room: " + state);
            //Lobby.DisconnectClient(startedBy, DisconnectCode.SuspiciousRequests);
            return;
        }

        state = State.started;

        SendToAll(RoomStartNotifyCmd.Notify(startedBy.id));
    }

    public bool TryChangeOwner(Client newOwner)
    {
        if (!clients.Contains(newOwner))
        {
            Debug.LogWarning("Tried to set a new owner that is not in the room.");
            return false;
        }

        owner = newOwner;

        SendToAll(RoomCreaterChangeNotifyCmd.Notify(owner.id));

        return true;
    }

#pragma warning disable CA1822
    // in TLCK its used to sync coins and etc instead...
    public void Lock(Client client, string pwd)
    {
        /*if (owner != client)
        {
            // ur not owner lil bro
            //_ = LobbyUtils.SendToClient(RoomLockResCmd.Response(false), client);
            //return;
        }*/

        //SendToAll(RoomLockResCmd.Response(true, pwd));

        _ = LobbyUtils.SendToClient(RoomLockResCmd.Response(true, pwd), client);

        //password = pwd;
    }
#pragma warning restore

    public void Rename(string newName, Client client)
    {
        /*if (client != owner)
        {
            // ur not owner lil bro
            //return;
        }*/

        name = newName;

        Debug.LogInfo("Set room name to: " + name);

        SendToAll(RoomRenameNotifyCmd.Notify(client.id, newName));
    }
    /*public void Unlock(Client client, string pwd)
    {

    }*/

    public void Dispose() => ShutDown();
}
