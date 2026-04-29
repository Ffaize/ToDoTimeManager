using System.ComponentModel.DataAnnotations;

namespace ToDoTimeManager.Shared.DTOs;

public class SendTwoFactorCodeRequestDto
{
    [Required]
    public Guid UserId { get; set; }
}
