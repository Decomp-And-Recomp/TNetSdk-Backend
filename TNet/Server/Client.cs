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
    public bool disconnected { get; private set; }

    public Client(TcpClient client)
    {
        connection = client;

        _ = Loop();
    }

    async Task Loop()
    {
        while (!disconnected)
        {
            missedHeartbeatCounter++;

            if (missedHeartbeatCounter > 30 || !connection.Connected)
            {
                Debug.Log("Client havent sent anything in a while, removing..");
                Disconnect();
                break;
            }

            await Task.Delay(2000);
        }
    }

    public void Disconnect()
    {
        if (disconnected) return;

        disconnected = true;

        Debug.Log("Removing: " + id, ConsoleColor.DarkRed);
        Debug.LogStack();

        if (Lobby.clients.TryRemove(id, out var removed))
        {
            if (removed != null && removed != this)
                Debug.Log("We deadass just disconnected random player 💀", ConsoleColor.DarkMagenta);
        }
        else
        {
            Debug.Log("Player wasnt removed from dictionary properly.", ConsoleColor.DarkRed);
        }

        RemoveFromRoom();

        connection?.Close();
        connection?.Dispose();
    }

    public void SetUserVar(ushort key, byte[] var)
    {
        vars[key] = var;

        if (room == null) return;

        room.SendToAll(RoomUserVarNotifyCmd.Notify(id, key, var));
    }

    public void RemoveFromRoom()
    {
        if (room == null) return;

        room.RemoveClient(this, false);
    }

    public void Dispose() => Disconnect();
}
