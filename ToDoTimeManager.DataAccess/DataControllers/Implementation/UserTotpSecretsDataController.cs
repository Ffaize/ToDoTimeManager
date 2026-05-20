using Dapper;
using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.DataAccess.DbAccessServices;
using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Implementation;

public class UserTotpSecretsDataController : IUserTotpSecretsDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<UserTotpSecretsDataController> _logger;

    public UserTotpSecretsDataController(IDbAccessService dbAccessService, ILogger<UserTotpSecretsDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger          = logger;
    }

    public async Task<UserTotpSecretEntity?> GetByUserId(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetOneByParameter<UserTotpSecretEntity>(
                "sp_UserTotpSecrets_GetByUserId", "UserId", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> Upsert(Guid userId, string secret)
    {
        try
        {
            var p = new DynamicParameters();
            p.Add("UserId", userId);
            p.Add("Secret", secret);
            return await _dbAccessService.ExecuteByParameters("sp_UserTotpSecrets_Upsert", p) >= 0;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteByUserId(Guid userId)
    {
        try
        {
            var p = new DynamicParameters();
            p.Add("UserId", userId);
            return await _dbAccessService.ExecuteByParameters("sp_UserTotpSecrets_DeleteByUserId", p) >= 0;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
