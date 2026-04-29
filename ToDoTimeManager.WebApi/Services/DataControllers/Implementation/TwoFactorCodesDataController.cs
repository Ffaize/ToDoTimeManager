using Dapper;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation;

public class TwoFactorCodesDataController : ITwoFactorCodesDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<TwoFactorCodesDataController> _logger;

    public TwoFactorCodesDataController(IDbAccessService dbAccessService, ILogger<TwoFactorCodesDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }

    public async Task<bool> UpsertCode(TwoFactorCodeEntity entity)
    {
        try
        {
            return await _dbAccessService.AddRecord("sp_TwoFactorCodes_Upsert", entity) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<TwoFactorCodeEntity?> GetByUserId(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetOneByParameter<TwoFactorCodeEntity>(
                "sp_TwoFactorCodes_GetByUserId", "UserId", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> DeleteByUserId(Guid userId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            return await _dbAccessService.ExecuteByParameters("sp_TwoFactorCodes_DeleteByUserId", parameters) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
