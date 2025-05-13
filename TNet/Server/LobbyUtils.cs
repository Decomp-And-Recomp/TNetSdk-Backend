using System.Net;
using System.Net.Sockets;
using TNet.Encryption;
using TNet.Server.Binary;

namespace TNet.Server;

internal class LobbyUtils
{
    public static void Log(object message, ConsoleColor color = ConsoleColor.White)
    {
        Debug.Log("[Lobby] " + message, color);
    }

    public static void LogBadUnpacker(object message)
    {
        Debug.Log("[Lobby:Bad Unpacker] " + message, ConsoleColor.DarkRed);
    }

    public static void LogUnimpl(object message)
    {
        Debug.Log("[Lobby:Unimplemented] " + message, ConsoleColor.DarkRed);
    }
#pragma warning disable
    public static void LogNewConnection(Client client)
    {
        Log("New connection from: " + ((IPEndPoint)client.connection.Client.RemoteEndPoint).Address.ToString()
            + " id:" + client.id);
    }
#pragma warning restore
    public static void Decrypt(Packet packet, BlowFish fish)
    {
        ulong val = 0;

        packet.Position = 0;
        packet.PopUInt64(ref val);
        fish.Decrypt(ref val);
        packet.Position = 0;
        packet.PushUInt64(val);
    }

#pragma warning disable
    public static void Encrypt(Packet packet, BlowFish fish)
    {
        /*ulong val = 0;

        packet.Position = 0;
        packet.PopUInt64(ref val);
        fish.Encrypt(ref val);
        packet.Position = 0;
        packet.PushUInt64(val);*/
    }
#pragma warning restore

    public static async Task SendToClient(Packet packet, Client client)
    {
        Encrypt(packet, Lobby.blowFish);

        byte[] bytes = new byte[packet.Length];

        packet.Position = 0;

        if (!packet.PopByteArray(ref bytes, packet.Length - 1))
        {
            Debug.LogError("Packet Fail");
            return;
        }

        await client.connection.GetStream().WriteAsync(bytes);
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
}
