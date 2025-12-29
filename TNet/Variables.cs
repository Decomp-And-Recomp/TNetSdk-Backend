namespace TNet;

internal class Variables
{
    public static ushort MaxRooms = 250;
    public static ushort MaxClients = 500;

    public static ushort MinRoomValue = 1000;
    public static ushort MaxRoomValue = 9999;

    public static float HeartbeatTimeout = 10;

    public static string EncryptionKey = string.Empty;
    public static Version Version = Version.Gen1;
}
