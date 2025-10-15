using TNet.Protocols.Objects;

namespace TNet.Protocols.Room.Server;

internal class RoomCreateResCmd : IServerPacket
{
    public enum Result : ushort
    {
        Ok = 0,
        Full = 1
    }

    public Result result;
    public ushort roomId;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushUInt16((ushort)result);
        p.PushUInt16(roomId);

        return p.MakePacket(Cmd.CreateResponse);
    }
}
