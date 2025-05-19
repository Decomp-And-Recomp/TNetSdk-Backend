using TNet.Server;

namespace TNet;

internal static class AdminCommands
{
    public static void SetUp()
    {
        AdminPanel.RegisterCommand("client-list", ClientList, "Lists all clients.");
        AdminPanel.RegisterCommand("client-disconnect", ClientDisconnect, "Put id(s) after command.");
        AdminPanel.RegisterCommand("room-list", RoomList, "Lists all rooms.");
        AdminPanel.RegisterCommand("room-start", RoomStart, "Put id(s) after command.");
        AdminPanel.RegisterCommand("room-shut", RoomShut, "Put id(s) after command.");
    }

    static void ClientList(string[] arguments)
    {
        foreach (Client c in Lobby.clients.Values)
        {
            Debug.Log($"id:{c.id}, in_room:{c.room != null}, disconnected:{c.disconnected}, nick:{c.nickname} ");
        }
    }

    static void ClientDisconnect(string[] arguments)
    {
        foreach (string arg in arguments)
        {
            if (!ushort.TryParse(arg, out ushort result))
            {
                Debug.Log("Unable to parse: " + arg, ConsoleColor.Yellow);
                continue;
            }

            if (!Lobby.clients.ContainsKey(result))
            {
                Debug.Log("client list does not contain: " + arg, ConsoleColor.Yellow);
                continue;
            }

            Lobby.clients[result].Disconnect();
        }
    }

    static void RoomList(string[] arguments)
    {
        foreach (Room c in Lobby.rooms.Values)
        {
            if (c.owner != null) Debug.Log($"id:{c.id}, owner id:{c.owner.id}, client count:{c.clients.Count}");
            else Debug.Log($"id:{c.id}, OWNER IS NULL, client count:{c.clients.Count}", ConsoleColor.Yellow);
        }
    }

    static void RoomStart(string[] arguments)
    {
        foreach (string arg in arguments)
        {
            if (!ushort.TryParse(arg, out ushort result))
            {
                Debug.Log("Unable to parse: " + arg, ConsoleColor.Yellow);
                continue;
            }

            if (!Lobby.rooms.ContainsKey(result))
            {
                Debug.Log("room list does not contain: " + arg, ConsoleColor.Yellow);
                continue;
            }

            Debug.Log($"Starting room {arg}...");

            Lobby.rooms[result].Start();
        }
    }

    static void RoomShut(string[] arguments)
    {
        foreach (string arg in arguments)
        {
            if (!ushort.TryParse(arg, out ushort result))
            {
                Debug.Log("Unable to parse: " + arg, ConsoleColor.Yellow);
                continue;
            }

            if (!Lobby.rooms.ContainsKey(result))
            {
                Debug.Log("room list does not contain: " + arg, ConsoleColor.Yellow);
                continue;
            }

            Debug.Log($"Shutting room {arg}...");

            Lobby.rooms[result].ShutDown();
        }
    }
}
