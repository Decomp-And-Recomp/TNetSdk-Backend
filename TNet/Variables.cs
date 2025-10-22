namespace TNet;

internal class Variables
{
    public static ushort maxRooms = 250;
    public static ushort maxClients = 500;

    public static ushort minRoomValue = 1000;
    public static ushort maxRoomValue = 9999;

    public static float heartbeatTimeout = 10;

    public static string encryptionKey = string.Empty;
    public static Version version = Version.Gen1;
}
