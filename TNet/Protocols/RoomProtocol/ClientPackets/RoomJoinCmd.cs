namespace TNet.Protocols.RoomProtocol.ClientPackets;

internal class RoomJoinCmd : IClientPacket
{
    public ushort id;
    public string password = string.Empty;

    public bool Parse(UnPacker unPacker)
    {
        if (!unPacker.PopUInt16(ref id)) return false;
        if (!unPacker.PopString(ref password)) return false;

        return true;
    }
}
