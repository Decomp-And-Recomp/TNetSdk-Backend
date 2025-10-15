namespace TNet.Protocols.Room;

internal enum Cmd : ushort
{
    None = 0,
    DragList = 1,
    DragListResponse = 2,
    Create = 3,
    CreateResponse = 4,
    Destroy = 5,
    DestroyResponse = 6,
    DestroyNotify = 7,
    Join = 8,
    JoinResponse = 9,
    JoinNotify = 10,
    Leave = 11,
    LeaveNotify = 12,
    KickUser = 13,
    KickUserNotify = 14,
    Rename = 15,
    RenameNotify = 16,
    SetVar = 17,
    SetVarNotify = 18,
    SetUserVar = 19,
    SetUserVarNotify = 20,
    SetUserStatus = 21,
    SetUserStatusNotify = 22,
    SendMessage = 23,
    BroadcastMessage = 24,
    MessageNotify = 25,
    LockRequest = 26,
    LockResponse = 27,
    UnlockRequest = 28,
    UnlockResponse = 29,
    Start = 30,
    StartNotify = 31,
    ChangeOwnerNotify = 32,

    // give them proper names
    SetCreateParam = 33,
    SetCreateParamChange = 34
}
