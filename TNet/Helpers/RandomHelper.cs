namespace TNet.Helpers;

internal class RandomHelper
{
    static readonly Random random = new();

    public static ushort GetRoomId()
    {
        return (ushort)random.Next(Variables.minRoomValue, Variables.maxRoomValue);
    }

    public static ushort GetClientId()
    {
        return (ushort)random.Next(ushort.MinValue, ushort.MaxValue);
    }
}
