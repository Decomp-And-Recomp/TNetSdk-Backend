namespace TNet;

internal enum DisconnectCode
{
    Debug = 0,
    SocketDisconnect = 1,
    DataOutOfBounds = 2,
    HeartbeatTimeout = 3,
    PacketException = 4,
    ReadException = 5
}
