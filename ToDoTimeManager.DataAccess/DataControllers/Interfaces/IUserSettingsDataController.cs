using ToDoTimeManager.Entities.Entities;

namespace ToDoTimeManager.DataAccess.DataControllers.Interfaces;

public interface IUserSettingsDataController
{
    Task<UserSettingsEntity?> GetByUserId(Guid userId);
    Task<bool>                Update(UserSettingsEntity settings);
    Task<bool?>               GetTwoFactorEnabled(Guid userId);
    Task<bool>                SetTwoFactorEnabled(Guid userId, bool isEnabled);
    Task<int?>                GetTwoFactorMethod(Guid userId);
    Task<bool>                SetTwoFactorMethod(Guid userId, int method);
}
