using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations
{
    public class TimeLogsService : ITimeLogsService
    {
        private readonly ITimeLogsDataController _timeLogsDataController;
        private readonly ILogger<TimeLogsService> _logger;
        public TimeLogsService(ITimeLogsDataController timeLogsDataController, ILogger<TimeLogsService> logger)
        {
            _timeLogsDataController = timeLogsDataController;
            _logger = logger;
        }

        public async Task<List<TimeLog>> GetAllTimeLogs()
        {
            try
            {
                var res = await _timeLogsDataController.GetAllTimeLogs();
                return res.Select(tle => tle.ToTimeLog()).ToList()!;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return [];
            }
        }

        public async Task<TimeLog?> GetTimeLogById(Guid timeLogId)
        {
            try
            {
                var res = await _timeLogsDataController.GetTimeLogById(timeLogId);
                return res?.ToTimeLog();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<List<TimeLog>> GetTimeLogsByToDoId(Guid toDoId)
        {
            try
            {
                var res = await _timeLogsDataController.GetTimeLogsByToDoId(toDoId);
                return res.Select(tle => tle.ToTimeLog()).ToList()!;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return [];
            }
        }

        public async Task<List<TimeLog>> GetTimeLogsByUserId(Guid userId)
        {
            try
            {
                var res = await _timeLogsDataController.GetTimeLogsByUserId(userId);
                return res.Select(tle => tle.ToTimeLog()).ToList()!;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return [];
            }
        }

        public async Task<List<TimeLog>> GetTimeLogsByUserIdAndToDoId(Guid toDoId, Guid userId)
        {
            try
            {
                var res = await _timeLogsDataController.GetTimeLogsByUserIdAndToDoId(toDoId, userId);
                return res.Select(tle => tle.ToTimeLog()).ToList()!;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return [];
            }
        }

        public async Task<bool> CreateTimeLog(TimeLog newTimeLog)
        {
            try
            {
                return await _timeLogsDataController.CreateTimeLog(new TimeLogEntity(newTimeLog));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> UpdateTimeLog(TimeLog updatedTimeLog)
        {
            try
            {
                return await _timeLogsDataController.UpdateTimeLog(new TimeLogEntity(updatedTimeLog));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteTimeLog(Guid timeLogId)
        {
            try
            {
                return await _timeLogsDataController.DeleteTimeLog(timeLogId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }
    }
}
