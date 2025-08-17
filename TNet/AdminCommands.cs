using TNet.Server;

namespace TNet;

internal static class AdminCommands
{
    public static void SetUp()
    {
        AdminPanel.RegisterCommand("GCC", (s) => { GC.Collect(); }, "Calls System.GC.Collect().");
        AdminPanel.RegisterCommand("client-list", ClientList, "Lists all clients. -i to reveal IPs");
        AdminPanel.RegisterCommand("client-disconnect", ClientDisconnect, "Disconnects a client(s), Put id(s) after command.");
        AdminPanel.RegisterCommand("room-list", RoomList, "Lists all rooms.");
        AdminPanel.RegisterCommand("room-start", RoomStart, "Starts a room(s). Put id(s) after command.");
        AdminPanel.RegisterCommand("room-shut", RoomShut, "Shuts a room(s). Put id(s) after command.");
    }

    static void ClientList(string[] arguments)
    {
        Debug.Write("id|room id|disconnected|heartbeat counter|nick|IP");
        if (arguments.Contains("-i"))
        {
            foreach (Client c in Lobby.clients.Values)
            {
                if (c.room != null) Debug.Write($"{c.id}|{c.room.id}|{c.disconnected}|{c.missedHeartbeatCounter}|\"{c.nickname}\"|{c.ipAddress}");
                else Debug.Write($"{c.id}|N/A|{c.disconnected}|{c.missedHeartbeatCounter}|\"{c.nickname}\"|{c.ipAddress}");
            }
            return;
        }

        foreach (Client c in Lobby.clients.Values)
        {
            if (c.room != null) Debug.Write($"{c.id}|{c.room.id}|{c.disconnected}|{c.missedHeartbeatCounter}|\"{c.nickname}\"|X");
            else Debug.Write($"{c.id}|N/A|{c.disconnected}|{c.missedHeartbeatCounter}|\"{c.nickname}\"|X");
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

            if (!Lobby.clients.TryGetValue(result, out Client? theClient))
            {
                Debug.Write("client list does not contain: " + arg, ConsoleColor.Yellow);
                continue;
            }

            theClient.Disconnect();
        }
    }

    static void RoomList(string[] arguments)
    {
        Debug.Write("id|owner id|client count|state");
        foreach (Room c in Lobby.rooms.Values)
        {
            if (c.owner != null) Debug.Write($"{c.id}|{c.owner.id}|{c.clients.Count}|{c.state}");
            else Debug.Write($"id:{c.id}|N/A|{c.clients.Count}|{c.state}", ConsoleColor.Red);
        }
    }

    static void RoomStart(string[] arguments)
    {
        foreach (string arg in arguments)
        {
            if (!ushort.TryParse(arg, out ushort result))
            {
                Debug.Write("Unable to parse: " + arg, ConsoleColor.Yellow);
                continue;
            }

            if (!Lobby.rooms.TryGetValue(result, out Room? theRoom))
            {
                Debug.Write("room list does not contain: " + arg, ConsoleColor.Yellow);
                continue;
            }

            Debug.Write($"Starting room {arg}...");

            theRoom.Start();
        }
    }

    static void RoomShut(string[] arguments)
    {
        foreach (string arg in arguments)
        {
            if (!ushort.TryParse(arg, out ushort result))
            {
                Debug.Write("Unable to parse: " + arg, ConsoleColor.Yellow);
                continue;
            }

            if (!Lobby.rooms.TryGetValue(result, out Room? theRoom))
            {
                Debug.Write("room list does not contain: " + arg, ConsoleColor.Yellow);
                continue;
            }

            Debug.Write($"Shutting room {arg}...");

            theRoom.ShutDown();
        }
    }
}
