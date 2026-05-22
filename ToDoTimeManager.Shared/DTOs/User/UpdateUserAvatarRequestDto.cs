using System.ComponentModel.DataAnnotations;

namespace ToDoTimeManager.Shared.DTOs.User;

public class UpdateUserAvatarRequestDto
{
    [Required]
    public required string Avatar { get; set; }
}
