namespace ToDoTimeManager.Entities.Exceptions;

public class ConflictException : ServiceException
{
    public ConflictException(string message) : base(409, message)
    {
    }
}
