namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomUserVarNotifyCmd : IServerPacket
{
    public ushort userId;
    public ushort key;
    public byte[]? data;

    public byte[] Pack()
    {
        if (data == null) throw new ArgumentNullException("data");

        Packer p = new();
        p.PushUInt16(userId);
        p.PushUInt16(key);
        p.PushUInt16((ushort)data.Length);
        p.PushByteArray(data);

        return p.MakePacket(Cmd.SetUserVarNotify);
    }
}
