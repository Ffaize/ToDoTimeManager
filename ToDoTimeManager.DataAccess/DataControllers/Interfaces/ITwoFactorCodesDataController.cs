using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Interfaces;

public interface ITwoFactorCodesDataController
{
    Task<bool> UpsertCode(TwoFactorCodeEntity entity);
    Task<TwoFactorCodeEntity?> GetByUserId(Guid userId);
    Task<bool> DeleteByUserId(Guid userId);
}
