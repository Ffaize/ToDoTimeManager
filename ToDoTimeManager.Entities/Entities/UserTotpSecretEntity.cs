namespace ToDoTimeManager.Entities.Entities;

public class UserTotpSecretEntity
{
    public Guid   UserId { get; set; }
    public string Secret { get; set; } = string.Empty;
}
