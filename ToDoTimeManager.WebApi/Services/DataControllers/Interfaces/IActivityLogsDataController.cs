using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface IActivityLogsDataController
{
    Task<List<ActivityLogEntity>> GetAllActivityLogs();
    Task<ActivityLogEntity?>      GetActivityLogById(Guid id);
    Task<List<ActivityLogEntity>> GetActivityLogsByToDoId(Guid toDoId);
    Task<List<ActivityLogEntity>> GetActivityLogsByUserId(Guid userId);
    Task<List<ActivityLogEntity>> GetActivityLogsByUserIdAndToDoId(Guid userId, Guid toDoId);
    Task<bool>                    CreateActivityLog(ActivityLogEntity entry);
    Task<bool>                    DeleteActivityLog(Guid id);
}
