using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.DTOs.User;

public sealed class UserResponseDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public UserRole? UserRole { get; set; }
}