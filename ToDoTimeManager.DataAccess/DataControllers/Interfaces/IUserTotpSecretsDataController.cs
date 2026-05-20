using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Interfaces;

public interface IUserTotpSecretsDataController
{
    Task<UserTotpSecretEntity?> GetByUserId(Guid userId);
    Task<bool>                  Upsert(Guid userId, string secret);
    Task<bool>                  DeleteByUserId(Guid userId);
}
