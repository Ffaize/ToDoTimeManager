using System.ComponentModel.DataAnnotations;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs;

public sealed class CreateUserRequestDto
{
    [NotEmptyGuid] public Guid Id { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    [Required] public UserRole? UserRole { get; set; }
}