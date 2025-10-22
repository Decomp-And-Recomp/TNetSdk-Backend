namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomUnlockResCmd : IServerPacket
{
    public bool success;
    public string key = string.Empty;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt16(success ? (ushort)0 : (ushort)1);
        p.PushString(key);

        return p.MakePacket(Cmd.UnlockResponse);
    }
}
