namespace TNet.Server;

internal enum DisconnectCode
{
    NoCode = 0,
    TooMuchData = 1,
    BadProtocol = 2,
    UnknownCommand = 3,
    CouldntAddToDictionary = 4
}
