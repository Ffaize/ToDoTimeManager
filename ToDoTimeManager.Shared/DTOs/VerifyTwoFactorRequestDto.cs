using System.ComponentModel.DataAnnotations;

namespace ToDoTimeManager.Shared.DTOs;

public class VerifyTwoFactorRequestDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string? Code { get; set; }
}
