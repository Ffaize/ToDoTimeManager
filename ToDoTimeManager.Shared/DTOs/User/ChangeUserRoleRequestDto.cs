using System.ComponentModel.DataAnnotations;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.DTOs.User;

public sealed class ChangeUserRoleRequestDto
{
    [Required] public UserRole NewRole { get; set; }
}