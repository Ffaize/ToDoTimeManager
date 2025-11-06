using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface ITimeLogsDataController
{
    Task<List<TimeLogEntity>> GetAllTimeLogs();
    Task<TimeLogEntity?> GetTimeLogById(Guid timeLogId);
    Task<List<TimeLogEntity>> GetTimeLogsByToDoId(Guid toDoId);
    Task<List<TimeLogEntity>> GetTimeLogsByUserId(Guid userId);
    Task<List<TimeLogEntity>> GetTimeLogsByUserIdAndToDoId(Guid toDoId, Guid userId);
    Task<bool> CreateTimeLog(TimeLogEntity newTimeLog);
    Task<bool> UpdateTimeLog(TimeLogEntity updatedTimeLog);
    Task<bool> DeleteTimeLog(Guid timeLogId);

}