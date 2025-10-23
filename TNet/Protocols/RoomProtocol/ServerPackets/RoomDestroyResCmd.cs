namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomDestroyResCmd : IServerPacket
{
    public bool success;
    public byte[] Pack()
    {
        Packer p = new();

        p.PushUInt16(success ? (ushort)1 : (ushort)0);

        return p.MakePacket(Cmd.DestroyResponse);
    }
}
