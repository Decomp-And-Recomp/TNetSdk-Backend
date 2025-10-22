namespace TNet.Protocols.SystemProtocol.ServerPackets;

internal class HeartbeatCmdRes : IServerPacket
{
    public long m_server_time;

    public byte[] Pack()
    {
        Packer p = new();
        p.PushInt64(m_server_time);

        return p.MakePacket(Cmd.HeartbeatResponse);
    }
}
