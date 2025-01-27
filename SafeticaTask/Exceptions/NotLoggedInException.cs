namespace SafeticaTask.Exceptions;

public class NotLoggedInException : Exception
{
    public NotLoggedInException()
    {
    }

    public NotLoggedInException(string? message) : base(message)
    {
    }

    public NotLoggedInException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}