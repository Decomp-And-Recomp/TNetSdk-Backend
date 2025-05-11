using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TNet.Server;

internal class Client(TcpClient client) : IDisposable
{
    public readonly TcpClient connection = client;

    public bool isLogged;

    public string nickname = string.Empty; // i hate warnings
    public ushort id;

    public void Disconnect()
    {
        Lobby.clients.Remove(id);

        Dispose();
    }

    public void Dispose()
    {
        connection?.Close();
        connection?.Dispose();
    }
}
