namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomCreateResCmd : IServerPacket
{
    public bool success;
    public ushort roomId;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt16(success ? (ushort)0 : (ushort)1);
        p.PushUInt16(roomId);

        return p.MakePacket(Cmd.CreateResponse);
    }
}
