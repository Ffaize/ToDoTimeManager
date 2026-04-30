namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IAccessControlService
{
    Task<bool> CanAccessProject(Guid userId, Guid projectId);
    Task<bool> CanAccessToDo(Guid userId, Guid toDoId);
    Task<bool> CanAccessTimeLog(Guid userId, Guid timeLogId);
}
