namespace ToDoTimeManager.WebApi.Exceptions;

public class ValidationException : ServiceException
{
    public ValidationException(string message) : base(400, message)
    {
    }
}