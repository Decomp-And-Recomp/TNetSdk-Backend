namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomDestroyNotifyCmd : IServerPacket
{
    public byte[] Pack()
    {
        Packer p = new();

        return p.MakePacket(Cmd.DestroyNotify);
    }
}
