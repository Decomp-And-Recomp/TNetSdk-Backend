namespace TNet.Protocols.Common;

internal enum Cmd : ushort
{
    None = 0,
    Heartbeat = 1,
    HeartbeatResponse = 2,
    Login = 3,
    LoginResponse = 4,
    Logout = 5
}
