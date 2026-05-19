using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.DataAccess.DbAccessServices;
using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Implementation;

public class PasswordResetsDataController : IPasswordResetsDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<PasswordResetsDataController> _logger;

    public PasswordResetsDataController(IDbAccessService dbAccessService, ILogger<PasswordResetsDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }

    public async Task<bool> Upsert(PasswordResetEntity entity)
    {
        try
        {
            return await _dbAccessService.AddRecord("sp_PasswordResets_Upsert", entity) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<PasswordResetEntity?> GetByUserId(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetOneByParameter<PasswordResetEntity>(
                "sp_PasswordResets_GetByUserId", "UserId", userId);
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
            return await _dbAccessService.ExecuteByParameters("sp_PasswordResets_DeleteByUserId", parameters) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
