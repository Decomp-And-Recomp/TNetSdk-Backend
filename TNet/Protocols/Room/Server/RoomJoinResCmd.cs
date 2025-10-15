using TNet.Protocols.Objects;

namespace TNet.Protocols.Room.Server;

internal class RoomJoinResCmd : IServerPacket
{
    public enum Result : ushort
    {
        Ok = 0,
        Full = 1,
        DoesntExist = 2,
        AlreadyStarted = 3,
        PasswordMismatch = 4
    }

    public Result result;
    public ushort index;
    public RoomInfo roomInfo = new();

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt16((ushort)result);
        p.PushUInt16(index);
        roomInfo.ToWriter(p);

        return p.MakePacket(Cmd.JoinResponse);
    }
}
