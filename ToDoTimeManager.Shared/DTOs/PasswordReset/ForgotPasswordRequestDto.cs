using System.ComponentModel.DataAnnotations;

namespace ToDoTimeManager.Shared.DTOs.PasswordReset;

public class ForgotPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
