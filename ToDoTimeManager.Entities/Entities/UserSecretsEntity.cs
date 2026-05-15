namespace ToDoTimeManager.Entities.Entities;

public class UserSecretsEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PasswordSalt { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
}
