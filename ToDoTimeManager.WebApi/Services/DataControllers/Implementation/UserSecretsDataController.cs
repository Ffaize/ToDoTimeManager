using Dapper;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation;

public class UserSecretsDataController : IUserSecretsDataController
{
    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<UserSecretsDataController> _logger;

    public UserSecretsDataController(IDbAccessService dbAccessService, ILogger<UserSecretsDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }

    public async Task<UserSecretsEntity?> GetByUserId(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetOneByParameter<UserSecretsEntity>(
                "sp_UsersSecrets_GetByUserId", "UserId", userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> Create(UserSecretsEntity entity)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id);
            parameters.Add("UserId", entity.UserId);
            parameters.Add("PasswordSalt", entity.PasswordSalt);
            return await _dbAccessService.ExecuteByParameters("sp_UsersSecrets_Create", parameters) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateRefreshToken(Guid userId, string? refreshTokenHash, DateTime? expiresAt)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("RefreshToken", refreshTokenHash);
            parameters.Add("RefreshTokenExpiresAt", expiresAt);
            return await _dbAccessService.ExecuteByParameters("sp_UsersSecrets_UpdateRefreshToken", parameters) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdatePasswordSalt(Guid userId, string passwordSalt)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("PasswordSalt", passwordSalt);
            return await _dbAccessService.ExecuteByParameters("sp_UsersSecrets_UpdatePasswordSalt", parameters) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> ClearRefreshToken(Guid userId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            return await _dbAccessService.ExecuteByParameters("sp_UsersSecrets_ClearRefreshToken", parameters) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}
