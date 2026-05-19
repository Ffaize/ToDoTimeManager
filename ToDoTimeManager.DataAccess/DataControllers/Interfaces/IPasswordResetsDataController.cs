using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Interfaces;

public interface IPasswordResetsDataController
{
    Task<bool> Upsert(PasswordResetEntity entity);
    Task<PasswordResetEntity?> GetByUserId(Guid userId);
    Task<bool> DeleteByUserId(Guid userId);
}
