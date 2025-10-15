namespace TNet.Protocols.Common.Client;

internal class LoginCmd : IClientPacket
{
    public string account = string.Empty;
    public string password = string.Empty;
    public string nickname = string.Empty;

    public bool Parse(UnPacker unPacker)
    {
        if (!unPacker.PopString(ref account)) return false;
        if (!unPacker.PopString(ref password)) return false;
        if (!unPacker.PopString(ref nickname)) return false;

        return true;
    }
}
