using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation
{
    public class TimeLogsDataController : ITimeLogsDataController
    {
        public async Task<List<TimeLogEntity>> GetAllTimeLogs()
        {
            throw new NotImplementedException();
        }

        public async Task<TimeLogEntity?> GetTimeLogById(Guid timeLogId)
        {
            throw new NotImplementedException();
        }

        public async Task<TimeLogEntity> CreateTimeLog(TimeLogEntity newTimeLog)
        {
            throw new NotImplementedException();
        }

        public async Task<TimeLogEntity?> UpdateTimeLog(TimeLogEntity updatedTimeLog)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteTimeLog(Guid timeLogId)
        {
            throw new NotImplementedException();
        }
    }
}
