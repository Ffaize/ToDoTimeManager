using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IAccessControlService
{
    Task<bool> CanAccessProject(Guid userId, UserRole userRole, Guid projectId);
    Task<bool> CanAccessToDo(Guid userId, UserRole userRole, Guid toDoId);
    Task<bool> CanAccessTimeLog(Guid userId, UserRole userRole, Guid timeLogId);
}
