using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.DataAccess.DbAccessServices;

namespace ToDoTimeManager.DataAccess.DataControllers.Implementation;

public class AccessControlDataController : IAccessControlDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<AccessControlDataController> _logger;

    public AccessControlDataController(IDbAccessService dbAccessService, ILogger<AccessControlDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }

    public async Task<bool> CanAccessProject(Guid userId, Guid projectId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("ProjectId", projectId);
            List<bool> result = await _dbAccessService.GetRecordsByParameters<bool>(
                "sp_AccessControl_CanUserAccessProject", parameters);
            return result.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessToDo(Guid userId, Guid toDoId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("ToDoId", toDoId);
            List<bool> result = await _dbAccessService.GetRecordsByParameters<bool>(
                "sp_AccessControl_CanUserAccessToDo", parameters);
            return result.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessTimeLog(Guid userId, Guid timeLogId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("TimeLogId", timeLogId);
            List<bool> result = await _dbAccessService.GetRecordsByParameters<bool>(
                "sp_AccessControl_CanUserAccessTimeLog", parameters);
            return result.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> CanAccessTeam(Guid userId, Guid teamId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("TeamId", teamId);
            List<bool> result = await _dbAccessService.GetRecordsByParameters<bool>(
                "sp_AccessControl_CanUserAccessTeam", parameters);
            return result.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
