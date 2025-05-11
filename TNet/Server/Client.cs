using System.Net.Sockets;

namespace TNet.Server;

internal class Client(TcpClient client) : IDisposable
{
    public readonly TcpClient connection = client;

    public bool isLogged;

    public string nickname = string.Empty; // i hate warnings
    public ushort id;

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
            LobbyUtils.Log("S", ConsoleColor.DarkRed);
        }

        Dispose();
    }

    public void Dispose()
    {
        connection?.Close();
        connection?.Dispose();
    }
}
