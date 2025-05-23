namespace TNet.Server;

internal enum DisconnectCode
{
    NoCode,
    TooMuchData,
    UnknownProtocol,
    UnknownCommand,
    CouldntAddToDictionary,
    SuspiciousRequests,
    RoomLeave,
    SocketDisconnect,
    TooShortData,
    RoomKick,
    ReadException
}
