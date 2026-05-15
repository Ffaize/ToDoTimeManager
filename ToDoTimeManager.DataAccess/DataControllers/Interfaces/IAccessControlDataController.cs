namespace ToDoTimeManager.DataAccess.DataControllers.Interfaces;

public interface IAccessControlDataController
{
    Task<bool> CanAccessProject(Guid userId, Guid projectId);
    Task<bool> CanAccessToDo(Guid userId, Guid toDoId);
    Task<bool> CanAccessTimeLog(Guid userId, Guid timeLogId);
    Task<bool> CanAccessTeam(Guid userId, Guid teamId);
}
