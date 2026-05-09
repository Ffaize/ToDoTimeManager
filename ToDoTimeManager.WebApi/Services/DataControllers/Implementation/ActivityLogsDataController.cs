using Dapper;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation;

public class ActivityLogsDataController : IActivityLogsDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<ActivityLogsDataController> _logger;

    public ActivityLogsDataController(IDbAccessService dbAccessService, ILogger<ActivityLogsDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger          = logger;
    }

    public async Task<List<ActivityLogEntity>> GetAllActivityLogs()
    {
        try
        {
            return await _dbAccessService.GetAllRecords<ActivityLogEntity>("sp_ActivityLogs_GetAll");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<ActivityLogEntity?> GetActivityLogById(Guid id)
    {
        try
        {
            return await _dbAccessService.GetRecordById<ActivityLogEntity>("sp_ActivityLogs_GetById", id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<ActivityLogEntity>> GetActivityLogsByToDoId(Guid toDoId)
    {
        try
        {
            return await _dbAccessService.GetAllByParameter<ActivityLogEntity>("sp_ActivityLogs_GetByToDoId", "ToDoId", toDoId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<ActivityLogEntity>> GetActivityLogsByUserId(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetAllByParameter<ActivityLogEntity>("sp_ActivityLogs_GetByUserId", "UserId", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<ActivityLogEntity>> GetActivityLogsByUserIdAndToDoId(Guid userId, Guid toDoId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("ToDoId", toDoId);
            return await _dbAccessService.GetRecordsByParameters<ActivityLogEntity>(
                "sp_ActivityLogs_GetByUserIdAndToDoId", parameters);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<bool> CreateActivityLog(ActivityLogEntity entry)
    {
        try
        {
            return await _dbAccessService.AddRecord("sp_ActivityLogs_Create", entry) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteActivityLog(Guid id)
    {
        try
        {
            return await _dbAccessService.DeleteRecordById("sp_ActivityLogs_DeleteById", id) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
