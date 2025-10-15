namespace TNet.Protocols;

internal class DummyProtocolHandler : ProtocolHandler
{
    public override void Handle(Client client, UnPacker unPacker)
    {
        throw new NotSupportedException();
    }
}
