namespace TNet.Exceptions;

internal class MissingRoomOwnerException : Exception
{
    public MissingRoomOwnerException()
    { }

    public MissingRoomOwnerException(string? message)
        : base(message)
    { }

    public MissingRoomOwnerException(string? message, Exception? innerException)
        : base(message, innerException)
    { }
}
