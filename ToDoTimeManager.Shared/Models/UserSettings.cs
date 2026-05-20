using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models;

public class UserSettings
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public TwoFactorMethod TwoFactorMethod { get; set; } = TwoFactorMethod.Email;
}
