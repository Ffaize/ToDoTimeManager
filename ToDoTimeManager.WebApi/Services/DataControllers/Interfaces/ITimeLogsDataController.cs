using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces
{
    public interface ITimeLogsDataController
    {
        Task<List<TimeLogEntity>> GetAllTimeLogs();
        Task<TimeLogEntity?> GetTimeLogById(Guid timeLogId);
        Task<bool> CreateTimeLog(TimeLogEntity newTimeLog);
        Task<bool> UpdateTimeLog(TimeLogEntity updatedTimeLog);
        Task<bool> DeleteTimeLog(Guid timeLogId);
    }
}
