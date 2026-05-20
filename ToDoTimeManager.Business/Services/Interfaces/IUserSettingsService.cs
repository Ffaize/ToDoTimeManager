using ToDoTimeManager.Shared.DTOs.UserSettings;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface IUserSettingsService
{
    Task<UserSettings?> GetUserSettings(Guid userId, Guid currentUserId, UserRole currentUserRole);
    Task<bool>          UpdateUserSettings(Guid userId, UpdateUserSettingsRequestDto request);
    Task<bool?>         GetTwoFactorEnabled(Guid userId, Guid currentUserId, UserRole currentUserRole);
    Task<bool>          SetTwoFactorEnabled(Guid userId, bool isEnabled);
    Task<TotpSetupResponseDto> GenerateTotpSetup(Guid userId);
    Task                       ConfirmTotpSetup(Guid userId, string secret, string code);
}
