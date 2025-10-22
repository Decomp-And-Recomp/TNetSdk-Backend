namespace TNet.Protocols.RoomProtocol.ClientPackets;

internal class RoomUnlockReqCmd : IClientPacket
{
    public string key = string.Empty;

    public bool Parse(UnPacker unPacker)
    {
        if (!unPacker.PopString(ref key)) return false;

        return true;
    }
}
