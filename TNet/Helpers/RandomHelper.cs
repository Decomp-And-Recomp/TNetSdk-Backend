namespace TNet.Helpers;

internal class RandomHelper
{
    private static readonly Random Random = new();

    public static ushort GetRoomId()
    {
        return (ushort)Random.Next(Variables.MinRoomValue, Variables.MaxRoomValue);
    }

    public static ushort GetClientId()
    {
        return (ushort)Random.Next(ushort.MinValue, ushort.MaxValue);
    }
}
