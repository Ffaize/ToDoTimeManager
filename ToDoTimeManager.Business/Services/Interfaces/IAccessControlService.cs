namespace ToDoTimeManager.Business.Services.Interfaces;

public interface IAccessControlService
{
    Task<bool> IsAccessibleToUser(Guid userId, Guid objectId, string methodName);
    Task<bool> CanAccessProject(Guid userId, Guid projectId);
    Task<bool> CanAccessToDo(Guid userId, Guid toDoId);
    Task<bool> CanAccessTimeLog(Guid userId, Guid timeLogId);
    Task<bool> CanAccessTeam(Guid userId, Guid teamId);
}
