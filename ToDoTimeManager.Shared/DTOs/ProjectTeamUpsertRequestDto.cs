using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs;

public sealed class ProjectTeamUpsertRequestDto
{
    [NotEmptyGuid]
    public Guid Id { get; set; }

    [NotEmptyGuid]
    public Guid ProjectId { get; set; }

    [NotEmptyGuid]
    public Guid TeamId { get; set; }
}
