using System.Net.Sockets;
using TNet.Server.Notifications;

namespace TNet.Server;

internal class Client : IDisposable
{
    public readonly TcpClient connection;
    public Room? room;

    public bool isLogged;

    public string nickname = string.Empty; // i hate warnings
    public ushort id;

    public Dictionary<ushort, byte[]> vars = [];

    // warn every second, destroy if havent sent anything in a while
    public int missedHeartbeatCounter = 0;

    public Client(TcpClient client)
    {
        connection = client;

        _ = Loop();
    }

    async Task Loop()
    {
        while (connection.Connected)
        {
            missedHeartbeatCounter++;

            if (missedHeartbeatCounter > 7)
            {
                LobbyUtils.Log("Client havent sent anything in a while, removing..");
                Disconnect();
                break;
            }

            await Task.Delay(1500);
        }
    }

    public void Disconnect()
    {
        LobbyUtils.Log("Removing: " + id, ConsoleColor.DarkRed);

        if (Lobby.clients.TryRemove(id, out var removed))
        {
            if (removed != null && removed != this)
                LobbyUtils.Log("We deadass just disconnected random player 💀", ConsoleColor.DarkMagenta);
        }
        else
        {
            LobbyUtils.Log("Player wasnt removed from dictionary properly.", ConsoleColor.DarkRed);
        }

        _ = RemoveFromRoom();

        Dispose();
    }

    public void SetUserVar(ushort key, byte[] var)
    {
        vars[key] = var;

        if (room == null) return;

        room.SendToAll(RoomUserVarNotifyCmd.Notify(id, key, var));
    }

    public async Task RemoveFromRoom()
    {
        if (room == null) return;

        await room.RemoveClient(this);
    }

    public void Dispose()
    {
        _ = RemoveFromRoom();

        connection?.Close();
        connection?.Dispose();
    }
}
