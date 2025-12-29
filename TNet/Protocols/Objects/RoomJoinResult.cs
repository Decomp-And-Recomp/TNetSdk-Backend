namespace TNet.Protocols.Objects;

internal enum RoomJoinResult : ushort
{
    Ok = 0,
    Full = 1,
    DoesntExist = 2,
    AlreadyStarted = 3,
    PasswordMismatch = 4
}
