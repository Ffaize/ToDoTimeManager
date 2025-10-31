using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces
{
    public interface ITimeLogsDataController
    {
        Task<List<TimeLogEntity>> GetAllTimeLogs();
        Task<TimeLogEntity?> GetTimeLogById(Guid timeLogId);
        Task<TimeLogEntity> CreateTimeLog(TimeLogEntity newTimeLog);
        Task<TimeLogEntity?> UpdateTimeLog(TimeLogEntity updatedTimeLog);
        Task<bool> DeleteTimeLog(Guid timeLogId);
    }
}
