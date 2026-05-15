using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface IActivityLogsService
{
    Task<List<ActivityLog>> GetAllActivityLogs();
    Task<List<ActivityLog>> GetActivityLogsByToDoId(Guid toDoId, Guid currentUserId, UserRole currentUserRole);
    Task<List<ActivityLog>> GetActivityLogsByUserId(Guid userId, Guid currentUserId, UserRole currentUserRole);
    Task<List<ActivityLog>> GetActivityLogsByUserIdAndToDoId(Guid toDoId, Guid userId, Guid currentUserId, UserRole currentUserRole);
    Task LogActivity(Guid? toDoId, Guid userId, ActivityType type, string description);
}
