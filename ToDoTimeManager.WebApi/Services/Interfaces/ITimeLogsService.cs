using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces
{
    public interface ITimeLogsService
    {
        Task<List<TimeLog>> GetAllTimeLogs();
        Task<TimeLog?> GetTimeLogById(Guid timeLogId);
        Task<List<TimeLog>> GetTimeLogsByToDoId(Guid toDoId);
        Task<List<TimeLog>> GetTimeLogsByUserId(Guid userId);
        Task<List<TimeLog>> GetTimeLogsByUserIdAndToDoId(Guid toDoId, Guid userId);
        Task<bool> CreateTimeLog(TimeLog newTimeLog);
        Task<bool> UpdateTimeLog(TimeLog updatedTimeLog);
        Task<bool> DeleteTimeLog(Guid timeLogId);
    }
}
