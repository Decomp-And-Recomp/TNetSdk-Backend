namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomJoinNotifyCmd : IServerPacket
{
    public ushort userId;
    public string nickname = string.Empty;
    public ushort index;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt16(userId);
        p.PushString(nickname);
        p.PushUInt16(index);

        return p.MakePacket(Cmd.JoinNotify);
    }
}
