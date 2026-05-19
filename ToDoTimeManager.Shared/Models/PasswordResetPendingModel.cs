namespace ToDoTimeManager.Shared.Models;

public class PasswordResetPendingModel
{
    public Guid UserId { get; set; }
    public string? MaskedEmail { get; set; }
    public int CodeLifetimeSeconds { get; set; }
}
