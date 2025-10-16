using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNet.Protocols.Room.Client;

namespace TNet;

internal class Room
{
    public ushort id { get; private set; }
    public Client owner = null!;

    public static Room? TryCreate(RoomCreateCmd cmd, Client client)
    {
        Room result = new();

        if (Lobby.rooms.Count >= Variables.maxRooms)
        {

            return null;
        }

        ushort id;

        while (true)
        {

        }

        return result;
    }
}
