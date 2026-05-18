namespace ToDoTimeManager.Entities.Entities;

public class UserSettingsEntity
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}
