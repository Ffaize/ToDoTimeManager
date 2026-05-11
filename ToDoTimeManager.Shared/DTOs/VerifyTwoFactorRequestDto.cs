using System.ComponentModel.DataAnnotations;
using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs;

public class VerifyTwoFactorRequestDto
{
    [NotEmptyGuid]
    public Guid UserId { get; set; }

    [Required]
    [RegularExpression(@"^\d{3}-\d{3}$", ErrorMessage = "Code must be in the format XXX-XXX.")]
    public string Code { get; set; } = string.Empty;

    public bool KeepSignedIn { get; set; } = true;
}
