using System.ComponentModel.DataAnnotations;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs.Team;

public sealed class TeamMemberUpsertRequestDto
{
    [NotEmptyGuid] public Guid Id { get; set; }

    [NotEmptyGuid] public Guid TeamId { get; set; }

    [NotEmptyGuid] public Guid UserId { get; set; }

    [Required] public TeamMemberRole Role { get; set; }
}