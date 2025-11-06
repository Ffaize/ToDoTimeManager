using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models;

public class User
{
    public required Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public UserRole? UserRole { get; set; }

}