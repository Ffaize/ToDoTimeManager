namespace ToDoTimeManager.Shared.DTOs.UserSettings;

public class UserSettingsResponseDto
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}
