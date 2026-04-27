namespace ToDoTimeManager.WebApi.Exceptions;

public class ServiceException : Exception
{
    public int StatusCode { get; }

    public ServiceException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}