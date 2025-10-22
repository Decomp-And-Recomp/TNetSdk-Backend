namespace TNet.Protocols.RoomProtocol.ClientPackets;

internal class RoomBroadcastMsgCmd : IClientPacket
{
    public byte[] data = null!;

    public bool Parse(UnPacker unPacker)
    {
        ushort l = 0;
        if (!unPacker.PopUInt16(ref l)) return false;

        data = new byte[l];
        if (!unPacker.PopByteArray(ref data)) return false;

        return true;
    }
}
