using System.ComponentModel.DataAnnotations;
using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs.PasswordReset;

public class ResetPasswordRequestDto
{
    [NotEmptyGuid]
    public Guid UserId { get; set; }

    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}
