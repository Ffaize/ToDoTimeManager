using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models;

public class TwoFactorPendingModel
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public int CodeLifetimeSeconds { get; set; }
    public string? SenderEmail { get; set; }
    public TwoFactorMethod TwoFactorMethod { get; set; } = TwoFactorMethod.Email;
}
