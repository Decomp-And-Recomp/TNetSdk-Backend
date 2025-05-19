namespace TNet.Exceptions;

internal class InvalidPacketLengthException : Exception
{
    public InvalidPacketLengthException()
    { }

    public InvalidPacketLengthException(string? message)
        : base(message)
    { }

    public InvalidPacketLengthException(string? message, Exception? innerException)
        : base(message, innerException)
    { }
}
