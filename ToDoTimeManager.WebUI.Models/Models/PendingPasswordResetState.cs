namespace ToDoTimeManager.WebUI.Models.Models;

public class PendingPasswordResetState
{
    public Guid UserId { get; set; }
    public string MaskedEmail { get; set; } = string.Empty;
    public int CodeLifetimeSeconds { get; set; }
}
