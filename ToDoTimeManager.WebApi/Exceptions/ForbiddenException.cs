namespace ToDoTimeManager.WebApi.Exceptions;

public class ForbiddenException : ServiceException
{
    public ForbiddenException(string message = "Access denied") : base(403, message)
    {
    }
}