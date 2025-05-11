namespace TNet.Server;

internal enum DisconnectCode
{
    NoCode = 0,
    TooMuchData = 1,
    UnknownProtocol = 2,
    UnknownCommand = 3,
    CouldntAddToDictionary = 4,
    SuspiciousRequests = 5,
}
