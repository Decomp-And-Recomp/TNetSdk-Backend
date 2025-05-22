using TNet.Server;

namespace TNet;

internal static class AdminCommands
{
    public static void SetUp()
    {
        AdminPanel.RegisterCommand("client-list", ClientList, "Lists all clients.");
        AdminPanel.RegisterCommand("client-disconnect", ClientDisconnect, "Disconnects a client(s), Put id(s) after command.");
        AdminPanel.RegisterCommand("room-list", RoomList, "Lists all rooms.");
        AdminPanel.RegisterCommand("room-start", RoomStart, "Starts a room(s). Put id(s) after command.");
        AdminPanel.RegisterCommand("room-shut", RoomShut, "Shuts a room(s). Put id(s) after command.");
    }

    static void ClientList(string[] arguments)
    {
        Debug.Write("id|room id|disconnected|heartbeat counter|nick");
        foreach (Client c in Lobby.clients.Values)
        {
            if (c.room != null) Debug.Write($"{c.id}|{c.room.id}|{c.disconnected}|{c.missedHeartbeatCounter}|{c.nickname}");
            else Debug.Write($"{c.id}|N/A|{c.disconnected}|{c.missedHeartbeatCounter}|{c.nickname}");
        }
    }

    static void ClientDisconnect(string[] arguments)
    {
        foreach (string arg in arguments)
        {
            if (!ushort.TryParse(arg, out ushort result))
            {
                Debug.Write("Unable to parse: " + arg, ConsoleColor.Yellow);
                continue;
            }

            if (!Lobby.clients.ContainsKey(result))
            {
                Debug.Write("client list does not contain: " + arg, ConsoleColor.Yellow);
                continue;
            }

            Lobby.clients[result].Disconnect();
        }
    }

    static void RoomList(string[] arguments)
    {
        Debug.Write("id|owner id|client count");
        foreach (Room c in Lobby.rooms.Values)
        {
            if (c.owner != null) Debug.Log($"{c.id}|{c.owner.id}|{c.clients.Count}");
            else Debug.Log($"id:{c.id}|N/A|{c.clients.Count}", ConsoleColor.Red);
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
