namespace ToDoTimeManager.Entities.Exceptions;

public class NotFoundException : ServiceException
{
    public NotFoundException(string message) : base(404, message)
    {
    }
}
