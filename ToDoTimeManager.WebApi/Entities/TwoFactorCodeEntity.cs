namespace ToDoTimeManager.WebApi.Entities;

public class TwoFactorCodeEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
