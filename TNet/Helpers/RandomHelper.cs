namespace TNet.Helpers;

internal class RandomHelper
{
    static Random random = new();

    public static ushort GetRoomId()
    {
        return (ushort)random.Next(ushort.MinValue, ushort.MaxValue);
    }

    public static ushort GetClientId()
    {
        return (ushort)random.Next(ushort.MinValue, ushort.MaxValue);
    }
}
