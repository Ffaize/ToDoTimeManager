using System.ComponentModel.DataAnnotations;
using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs;

public class VerifyTwoFactorRequestDto
{
    [NotEmptyGuid]
    public Guid UserId { get; set; }

    [Required]
    public string Code { get; set; } = string.Empty;

    public bool KeepSignedIn { get; set; } = true;
}
