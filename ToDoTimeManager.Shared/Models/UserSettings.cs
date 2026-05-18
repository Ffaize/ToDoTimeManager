namespace ToDoTimeManager.Shared.Models;

public class UserSettings
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}
