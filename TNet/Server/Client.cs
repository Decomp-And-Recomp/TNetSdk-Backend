using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TNet.Server;

internal class Client(TcpClient client)
{
    public readonly TcpClient connection = client;

    public bool isLogged;

    public string nickname;
    public ushort id;

    public void Disconnect()
    {
        Lobby.clients.Remove(this);
        connection.Dispose();
    }
}
