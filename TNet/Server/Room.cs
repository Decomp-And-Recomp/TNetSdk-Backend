using TNet.Server.Binary;
using TNet.Server.Data;
using TNet.Server.Notifications;
using TNet.Server.Requests;

namespace TNet.Server;

internal class Room : IDisposable, IAsyncDisposable
{
    public enum State { open, gaming, shuttingDown, close }

    public State state { get; private set; }

    public ushort id;

    public string name = string.Empty;
    public string comment = string.Empty;

    public Client? owner;

    public List<Client> clients = [];

    public RoomSwitchMasterType masterSwitchType;

    public RoomType roomType;

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
        room.masterSwitchType = cmd.roomSwitchMasterType;
        room.roomType = cmd.roomType;

        LobbyUtils.Log($"Created new room with: {room.id}, {room.masterSwitchType}, {room.roomType}", ConsoleColor.Cyan);

        room.owner = owner;
        room.clients.Add(owner);

        owner.room = room;

        room.state = State.open;

        return true;
    }

    public async Task ShutDown()
    {
        state = State.shuttingDown;
        Debug.LogInfo("Shutting a room down");

        foreach (Client client in clients) await client.RemoveFromRoom();

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

        clients.Remove(client);

        client.room = null;

        Packet p = RoomLeaveNotifyCmd.Notify(client.id);

        foreach (Client c in clients) await LobbyCmdImpl.SendToClient(p, c);

        if (owner != client && owner != null) return;

        if (masterSwitchType == RoomSwitchMasterType.Auto && clients.Count > 0)
        {
            if (TryChangeOwner(clients[0])) return;
        }

        Debug.LogInfo("No room owner.");

        await ShutDown();
    }

    public bool TryChangeOwner(Client newOwner)
    {
        if (clients.Contains(newOwner))
        {
            Debug.LogWarning("Tried to set a new owner that is not in the room.");
            return false;
        }

        owner = newOwner;

        Packet notification = RoomCreaterChangeNotifyCmd.Notify(owner.id);

        foreach (Client c in clients) _ = LobbyCmdImpl.SendToClient(notification, c);

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
