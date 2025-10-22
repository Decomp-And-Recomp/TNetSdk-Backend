namespace TNet.Protocols.RoomProtocol.ClientPackets;

internal class RoomSendMsgCmd : IClientPacket
{
    public ushort userId;
    public byte[] data = null!;

    public bool Parse(UnPacker unPacker)
    {
        if (!unPacker.PopUInt16(ref userId)) return false;

        ushort l = 0;
        if (!unPacker.PopUInt16(ref l)) return false;

        data = new byte[l];
        if (!unPacker.PopByteArray(ref data)) return false;

        return true;
    }
}
