namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomRenameNotifyCmd : IServerPacket
{
    public ushort userId;
    public string name = string.Empty;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt16(userId);
        p.PushString(name);

        return p.MakePacket(Cmd.RenameNotify);
    }
}
