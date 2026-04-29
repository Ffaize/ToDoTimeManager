namespace ToDoTimeManager.Shared.Models;

public class TwoFactorPendingModel
{
    public Guid UserId { get; set; }
    public string? MaskedEmail { get; set; }
}
