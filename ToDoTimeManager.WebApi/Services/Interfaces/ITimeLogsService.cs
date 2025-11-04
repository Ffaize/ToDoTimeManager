using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.Interfaces
{
    public interface ITimeLogsService
    {
        Task<List<TimeLog>> GetAllTimeLogs();
        Task<TimeLog?> GetTimeLogById(Guid timeLogId);
        Task<bool> CreateTimeLog(TimeLog newTimeLog);
        Task<bool> UpdateTimeLog(TimeLog updatedTimeLog);
        Task<bool> DeleteTimeLog(Guid timeLogId);
    }
}
