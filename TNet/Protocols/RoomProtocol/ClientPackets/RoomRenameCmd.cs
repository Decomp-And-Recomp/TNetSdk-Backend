namespace TNet.Protocols.RoomProtocol.ClientPackets;

internal class RoomRenameCmd : IClientPacket
{
    public string name = string.Empty;

    public bool Parse(UnPacker unPacker)
    {
        if (!unPacker.PopString(ref name)) return false;

        return true;
    }
}
