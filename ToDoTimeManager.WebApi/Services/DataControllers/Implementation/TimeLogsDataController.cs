using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.DbAccessServices;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation
{
    public class TimeLogsDataController : ITimeLogsDataController
    {
        private readonly IDbAccessService _dbAccessService;
        private readonly ILogger<TimeLogsDataController> _logger;
        public TimeLogsDataController(IDbAccessService dbAccessService, ILogger<TimeLogsDataController> logger)
        {
            _dbAccessService = dbAccessService;
            _logger = logger;
        }

        public async Task<List<TimeLogEntity>> GetAllTimeLogs()
        {
            try
            {
                return await _dbAccessService.GetAllRecords<TimeLogEntity>("sp_TimeLogs_GetAll");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return [];
            }
        }

        public async Task<TimeLogEntity?> GetTimeLogById(Guid timeLogId)
        {
            try
            {
                return await _dbAccessService.GetRecordById<TimeLogEntity>("sp_TimeLogs_GetById", timeLogId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<bool> CreateTimeLog(TimeLogEntity newTimeLog)
        {
            try
            {
                return await _dbAccessService.AddRecord("sp_TimeLogs_Create", newTimeLog) >= 1;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> UpdateTimeLog(TimeLogEntity updatedTimeLog)
        {
            try
            {
                return await _dbAccessService.UpdateRecord("sp_TimeLogs_Update", updatedTimeLog) >= 1;
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
                return await _dbAccessService.DeleteRecordById("sp_TimeLogs_Delete", timeLogId) >= 1;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }
    }
}
