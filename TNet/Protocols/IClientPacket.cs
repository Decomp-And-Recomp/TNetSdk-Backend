namespace TNet.Protocols;

internal interface IClientPacket
{
    public bool Parse(UnPacker unPacker);
}
