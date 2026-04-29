using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface ITwoFactorCodesDataController
{
    Task<bool> UpsertCode(TwoFactorCodeEntity entity);
    Task<TwoFactorCodeEntity?> GetByUserId(Guid userId);
    Task<bool> DeleteByUserId(Guid userId);
}
