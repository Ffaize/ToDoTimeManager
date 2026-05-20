using System.ComponentModel.DataAnnotations;

namespace ToDoTimeManager.Shared.DTOs.UserSettings;

public class ConfirmTotpSetupRequestDto
{
    [Required]
    public string Secret { get; set; } = string.Empty;

    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string Code { get; set; } = string.Empty;
}
