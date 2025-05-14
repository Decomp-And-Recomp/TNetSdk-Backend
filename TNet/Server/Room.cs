using TNet.Server.Binary;
using TNet.Server.Cmd;
using TNet.Server.Data;
using TNet.Server.Notifications;
using TNet.Server.Requests;

namespace TNet.Server;

internal class Room : IDisposable, IAsyncDisposable
{
    struct VariableSet
    {
        public ushort userId;
        public byte[] data;
    }

    public enum State { open, started, shuttingDown, close }

    public State state { get; private set; }

    public ushort id, maxUsers, groupId;

    public string name = string.Empty;
    public string comment = string.Empty;

    public Client? owner;

    public List<Client> clients = [];

    public RoomSwitchMasterType masterSwitchType;

    public RoomType roomType;

    public bool isFull => clients.Count >= maxUsers;

    readonly Dictionary<ushort, VariableSet> vars = [];

    // Dictionary<Client, VariableSet>, optimise it to empty the data when player leaves
    readonly Dictionary<ushort, VariableSet> userVars = [];

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

        LobbyUtils.Log($"Created new room with: id={room.id}, maxUsers={room.maxUsers}, {room.masterSwitchType}, {room.roomType}", ConsoleColor.Cyan);

        room.owner = owner;
        _ = room.ConnectClient(owner);

        room.state = State.open;

        return true;
    }

    public void SendToAll(Packet packet)
    {
        foreach (Client c in clients)
           _ = LobbyUtils.SendToClient(packet, c);
    }

    public void SendToAll(Packet packet, params Client[] excludeClients)
    {
        foreach (Client c in clients)
           if (!excludeClients.Contains(c)) _ = LobbyUtils.SendToClient(packet, c);
    }

    public async Task ConnectClient(Client client)
    {
        client.room = this;

        clients.Add(client);

        Packet joinRes = RoomJoinResCmd.Response(RoomJoinResult.ok, (ushort)clients.IndexOf(client), SerializedRoomInfo.FromRoom(this));

        await LobbyUtils.SendToClient(joinRes, client);

        SendToAll(RoomJoinNotifyCmd.Notify(client));

        // sending every player to new player manualy
        // then syncing other stuff
        for (int i = 0;  i < clients.Count; i++)
        {
            if (clients[i] == client) continue;

            await LobbyUtils.SendToClient(RoomJoinNotifyCmd.Notify(clients[i]), client);
        }

        foreach (var v in vars)
            _ = LobbyUtils.SendToClient(RoomVarNotifyCmd.Notify(v.Value.userId, v.Key, v.Value.data), client);

        foreach (var v in userVars)
            _ = LobbyUtils.SendToClient(RoomUserVarNotifyCmd.Notify(v.Value.userId, v.Key, v.Value.data), client);

        Debug.LogInfo("Client connected, count: " + clients.Count);

#pragma warning disable CS8604 //ToDo: remove
        if (clients.Count > 1 && state == State.open) Start(owner);
#pragma warning restore
    }

    public async Task ShutDown()
    {
        state = State.shuttingDown;
        Debug.LogInfo("Shutting a room down");

        foreach (Client client in clients) await client.RemoveFromRoom();

        if (Lobby.rooms.Remove(id, out var removedRoom))
        {
            // Put it back
            if (removedRoom != this && removedRoom != null) 
                Lobby.rooms[id] = removedRoom;
        }

        state = State.close;
    }

    public async Task RemoveClient(Client client)
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
        Packet p = RoomLeaveNotifyCmd.Notify(client.id);

        foreach (Client c in clients) await LobbyUtils.SendToClient(p, c);

        clients.Remove(client);

        client.room = null;

        if (owner != client && owner != null) return;

        if (masterSwitchType == RoomSwitchMasterType.Auto && clients.Count > 0)
        {
            if (TryChangeOwner(clients[0])) return;
        }

        Debug.LogInfo("No room owner.");

        await ShutDown();
    }

    public void SetRoomVariable(ushort userId, ushort key, byte[] var)
    {
        Debug.LogInfo("Set room variable");

        vars[key] = new()
        { 
            userId = userId,
            data = var
        };

        SendToAll(RoomVarNotifyCmd.Notify(userId, key, var));
    }

    public void SetUserVariable(ushort userId, ushort key, byte[] var)
    {
        Debug.LogInfo("Set user variable");

        userVars[key] = new()
        { 
            userId = userId,
            data = var
        };

        SendToAll(RoomUserVarNotifyCmd.Notify(userId, key, var));
    }

    public void Start(Client startedBy)
    {
        if (state != State.open)
        {
            Debug.LogWarning("Room has to be open, but the sate is: " + state);
            return;
        }

        if (!clients.Contains(startedBy))
        {
            Debug.LogWarning("Lobby started by someone not in the room: " + state);
            Lobby.DisconnectClient(startedBy, DisconnectCode.SuspiciousRequests);
            return;
        }

        state = State.started;

        Packet notification = RoomStartNotifyCmd.Notify(startedBy.id);

        SendToAll(notification);
    }

    public bool TryChangeOwner(Client newOwner)
    {
        if (!clients.Contains(newOwner))
        {
            Debug.LogWarning("Tried to set a new owner that is not in the room.");
            return false;
        }

        owner = newOwner;

        Packet notification = RoomCreaterChangeNotifyCmd.Notify(owner.id);

        foreach (Client c in clients) _ = LobbyUtils.SendToClient(notification, c);

        Debug.LogInfo("Changed lobby owner");

        return true;
    }

#pragma warning disable CA2012 // Not sure if its good idea tho.
    public void Dispose() => DisposeAsync().GetAwaiter().GetResult();
#pragma warning restore

    public async ValueTask DisposeAsync()
    {
        await ShutDown();

        GC.SuppressFinalize(this);
    }
}
