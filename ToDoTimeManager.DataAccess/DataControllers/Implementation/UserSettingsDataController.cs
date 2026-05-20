using Dapper;
using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.DataAccess.DbAccessServices;
using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Implementation;

public class UserSettingsDataController : IUserSettingsDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<UserSettingsDataController> _logger;

    public UserSettingsDataController(IDbAccessService dbAccessService, ILogger<UserSettingsDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger          = logger;
    }

    public async Task<UserSettingsEntity?> GetByUserId(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetOneByParameter<UserSettingsEntity>(
                "sp_UserSettings_GetByUserId", "UserId", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> Update(UserSettingsEntity settings)
    {
        try
        {
            return await _dbAccessService.UpdateRecord("sp_UserSettings_Update", settings) >= 0;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool?> GetTwoFactorEnabled(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetOneByParameter<bool?>(
                "sp_UserSettings_GetTwoFactorEnabled", "UserId", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> SetTwoFactorEnabled(Guid userId, bool isEnabled)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("IsEnabled", isEnabled);
            return await _dbAccessService.ExecuteByParameters("sp_UserSettings_SetTwoFactorEnabled", parameters) >= 0;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<int?> GetTwoFactorMethod(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetOneByParameter<int?>(
                "sp_UserSettings_GetTwoFactorMethod", "UserId", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> SetTwoFactorMethod(Guid userId, int method)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("TwoFactorMethod", method);
            return await _dbAccessService.ExecuteByParameters("sp_UserSettings_SetTwoFactorMethod", parameters) >= 0;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
