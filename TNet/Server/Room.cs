using TNet.Server.Cmd;
using TNet.Server.Data;

namespace TNet.Server;

internal class Room
{
    public ushort id;

    public string name;
    public string comment;

    public Client? owner;

    public List<Client> clients = [];

    public RoomSwitchMasterType masterSwitchType;

    public RoomType roomType;

    Room() { }

    public static bool TryCreate(RoomCreateCmd cmd, out Room room)
    {
        room = new();

        bool added = false;

        for (ushort i = 0; i < ushort.MaxValue; i++)
        {
            if (!Lobby.rooms.TryAdd(i, room)) continue;

            added = true;

            room.id = i;

            break;
        }

        if (!added) return false;

        room.name = cmd.roomName;
        room.comment = cmd.param;
        room.masterSwitchType = cmd.roomSwitchMasterType;
        room.roomType = cmd.roomType;

        LobbyUtils.Log($"Created new room with id: {room.id}", ConsoleColor.Cyan);

        return true;
    }
}
