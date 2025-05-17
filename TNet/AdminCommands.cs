using TNet.Server;

namespace TNet;

internal static class AdminCommands
{
    public static void SetUp()
    {
        AdminPanel.RegisterCommand("client-list", ClientList, "lists all clients.");
        AdminPanel.RegisterCommand("client-disconnect", ClientDisconnect, "put id(s) after command.");
    }

    static void ClientList(string[] arguments)
    {
        foreach (Client c in Lobby.clients.Values)
        {
            Debug.Log($"id:{c.id} in_room:{c.room != null} disconnected:{c.disconnected} nick:{c.nickname} ");
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
}
