using TNet.Protocols.Objects;

namespace TNet.Protocols.RoomProtocol.ServerPackets;

internal class RoomJoinResCmd : IServerPacket
{
    public RoomJoinResult result;
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
