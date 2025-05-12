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

        LobbyUtils.Log($"Created new room with id: {room.id}", ConsoleColor.Cyan);

        room.owner = owner;
        room.clients.Add(owner);

        owner.room = room;

        room.state = State.open;

        return true;
    }

    public async Task ShutDown()
    {
        state = State.shuttingDown;
        Debug.Log("Shutting down");

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
            owner = clients[0];
            return;
        }

        Debug.Log("No room owner.");

        await ShutDown();
    }
#pragma warning disable CA2012
    public void Dispose() => DisposeAsync().GetAwaiter().GetResult();
#pragma warning restore

    public async ValueTask DisposeAsync()
    {
        await ShutDown();

        GC.SuppressFinalize(this);
    }
}
