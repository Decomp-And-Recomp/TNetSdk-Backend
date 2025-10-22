namespace TNet.Protocols.SystemProtocol.ServerPackets;

internal class LoginCmdRes : IServerPacket
{
    public enum Result : ushort
    {
        Ok = 0,
        Error_Password = 1
    }

    public Result result;
    public ushort userId;
    public string nickname = string.Empty;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt16((ushort)result);
        p.PushUInt16(userId);
        p.PushString(nickname);

        return p.MakePacket(Cmd.LoginResponse);
    }
}
