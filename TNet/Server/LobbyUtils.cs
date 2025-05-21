using TNet.Encryption;
using TNet.Server.Binary;

namespace TNet.Server;

internal class LobbyUtils
{
    public static void LogBadUnpacker(object message)
    {
        Debug.Log("[Lobby:Bad Unpacker] " + message, ConsoleColor.DarkRed);
    }

    public static void LogUnimpl(object message)
    {
        Debug.Log("[Lobby:Unimplemented] " + message, ConsoleColor.DarkRed);
    }

    public static void LogNewConnection(Client client)
    {
        //Log("New connection from: " + ((IPEndPoint)client.connection.Client.RemoteEndPoint).Address.ToString()
        //    + " id:" + client.id);
        Debug.Log("New connection, id: " + client.id);
    }

    public static ushort WatchUInt16(byte[] data, int pos)
    {
        return (ushort)((data[pos] << 8) | data[pos + 1]);
    }

    public static void Decrypt(Packet packet)
    {
        if (Lobby.blowFish == null) return;

        ulong val = 0;

        packet.Position = 0;
        packet.PopUInt64(ref val);
        Lobby.blowFish.Decrypt(ref val);
        packet.Position = 0;
        packet.PushUInt64(val);
    }

    public static void Decrypt(ref byte[] data)
    {
        if (Lobby.blowFish == null) return;

        uint num = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
        uint num2 = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
        ulong num3 = num;
        num3 = (num3 << 32) + num2;
        Lobby.blowFish.Decrypt(ref num3);
        num = (uint)(num3 >> 32);
        num2 = (uint)num3;
        data[0] = (byte)(((num & 0xFF000000u) >> 24) & 0xFFu);
        data[1] = (byte)(((num & 0xFF0000) >> 16) & 0xFFu);
        data[2] = (byte)(((num & 0xFF00) >> 8) & 0xFFu);
        data[3] = (byte)(num & 0xFFu & 0xFFu);
        data[4] = (byte)(((num2 & 0xFF000000u) >> 24) & 0xFFu);
        data[5] = (byte)(((num2 & 0xFF0000) >> 16) & 0xFFu);
        data[6] = (byte)(((num2 & 0xFF00) >> 8) & 0xFFu);
        data[7] = (byte)(num2 & 0xFFu & 0xFFu);
    }

    public static void Encrypt(Packet packet)
    {
        return;

        if (Lobby.blowFish == null) return;

        byte[] data = new byte[8];

        packet.Position = 0;
        packet.PopByteArray(ref data, 8);

        uint num = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
        uint num2 = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
        ulong num3 = num;
        num3 = (num3 << 32) + num2;
        Lobby.blowFish.Encrypt(ref num3);
        num = (uint)(num3 >> 32);
        num2 = (uint)num3;
        data[0] = (byte)(((num & 0xFF000000u) >> 24) & 0xFFu);
        data[1] = (byte)(((num & 0xFF0000) >> 16) & 0xFFu);
        data[2] = (byte)(((num & 0xFF00) >> 8) & 0xFFu);
        data[3] = (byte)(num & 0xFFu & 0xFFu);
        data[4] = (byte)(((num2 & 0xFF000000u) >> 24) & 0xFFu);
        data[5] = (byte)(((num2 & 0xFF0000) >> 16) & 0xFFu);
        data[6] = (byte)(((num2 & 0xFF00) >> 8) & 0xFFu);
        data[7] = (byte)(num2 & 0xFFu & 0xFFu);

        packet.Position = 0;
        packet.PushByteArray(data);
    }

    /*
            uint num = (uint)((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]);
            uint num2 = (uint)((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
            ulong num3 = num;
            num3 = (num3 << 32) + num2;
            m_blow_fish.Decrypt(ref num3);
            num = (uint)(num3 >> 32);
            num2 = (uint)num3;
            data[0] = (byte)(((num & 0xFF000000u) >> 24) & 0xFFu);
            data[1] = (byte)(((num & 0xFF0000) >> 16) & 0xFFu);
            data[2] = (byte)(((num & 0xFF00) >> 8) & 0xFFu);
            data[3] = (byte)(num & 0xFFu & 0xFFu);
            data[4] = (byte)(((num2 & 0xFF000000u) >> 24) & 0xFFu);
            data[5] = (byte)(((num2 & 0xFF0000) >> 16) & 0xFFu);
            data[6] = (byte)(((num2 & 0xFF00) >> 8) & 0xFFu);
            data[7] = (byte)(num2 & 0xFFu & 0xFFu);
 */

    public static async Task SendToClient(Client c, params Packet[] packets)
    {
        List<Task> tasks = [];

        foreach (Packet packet in packets) 
            tasks.Add(SendToClient(packet, c));

        await Task.WhenAll(tasks);
    }

    public static async Task SendToClients(Packet packet, params Client[] clients)
    {
        List<Task> tasks = [];

        foreach (Client c in clients) 
            tasks.Add(SendToClient(packet, c));

        await Task.WhenAll(tasks);
    }

    public static async Task SendToClient(Packet packet, Client client)
    {
        if (client.disconnected) return;

        Encrypt(packet);

        byte[] bytes = new byte[packet.Length];

        packet.Position = 0;
        packet.PopByteArray(ref bytes, packet.Length);

        try
        {
            await client.connection.GetStream().WriteAsync(bytes);
        }
        catch (Exception ex)
        {
            Debug.LogException("Send failed: ", ex);
        }
    }
}
