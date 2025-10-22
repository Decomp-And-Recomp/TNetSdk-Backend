namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomLeaveNotifyCmd : IServerPacket
{
    public ushort userId;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt16(userId);

        return p.MakePacket(Cmd.LeaveNotify);
    }
}
