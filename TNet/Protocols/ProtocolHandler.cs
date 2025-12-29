namespace TNet.Protocols;

internal abstract class ProtocolHandler
{
    public abstract void Handle(Client client, UnPacker unPacker);
}
