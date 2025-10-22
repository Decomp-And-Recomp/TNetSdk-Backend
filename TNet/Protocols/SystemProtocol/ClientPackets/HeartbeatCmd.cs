namespace TNet.Protocols.SystemProtocol.ClientPackets;

internal class HeartbeatCmd : IClientPacket
{
    public ushort ping;

    public bool Parse(UnPacker unPacker)
    {
        if (!unPacker.PopUInt16(ref ping)) return false;

        return true;
    }
}
