using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface ITimeLogsService
{
    Task<List<TimeLog>> GetAllTimeLogs();
    Task<TimeLog?> GetTimeLogById(Guid timeLogId, Guid currentUserId, UserRole currentUserRole);
    Task<List<TimeLog>> GetTimeLogsByToDoId(Guid toDoId, Guid currentUserId, UserRole currentUserRole);

    Task<List<TimeLog>> GetTimeLogsByUserId(Guid userId, Guid currentUserId, UserRole currentUserRole);
    Task<List<TimeLog>> GetTimeLogsByUserIdAndToDoId(Guid toDoId, Guid userId, Guid currentUserId, UserRole currentUserRole);
    Task<List<TimeLog>> GetTimeLogsByUserIdAndTime(Guid userId, int daysAgo);
    Task<bool> CreateTimeLog(TimeLog newTimeLog, Guid currentUserId, UserRole currentUserRole);
    Task<bool> UpdateTimeLog(TimeLog updatedTimeLog, Guid currentUserId, UserRole currentUserRole);
    Task<bool> DeleteTimeLog(Guid timeLogId, Guid currentUserId, UserRole currentUserRole);
}
