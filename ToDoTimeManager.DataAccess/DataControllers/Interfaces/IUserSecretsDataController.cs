using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Interfaces;

public interface IUserSecretsDataController
{
    Task<UserSecretsEntity?> GetByUserId(Guid userId);
    Task<string?> GetPasswordSaltByUserId(Guid userId);
    Task<bool> Create(UserSecretsEntity entity);
    Task<bool> UpdateRefreshToken(Guid userId, string? refreshTokenHash, DateTime? expiresAt);
    Task<bool> UpdatePasswordSalt(Guid userId, string passwordSalt);
    Task<bool> ClearRefreshToken(Guid userId);
}
