namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomMsgNotifyCmd : IServerPacket
{
    public ushort userId;
    public byte[]? data;

    public byte[] Pack()
    {
        if (data == null) throw new ArgumentNullException("data");

        Packer p = new();
        p.PushUInt16(userId);
        p.PushUInt16((ushort)data.Length);
        p.PushByteArray(data);

        return p.MakePacket(Cmd.MessageNotify);
    }
}
