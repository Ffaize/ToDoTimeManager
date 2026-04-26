using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Entities;

public class ProjectTeamEntity
{
    public ProjectTeamEntity() { }

    public Guid Id        { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TeamId    { get; set; }

    public ProjectTeam ToProjectTeam() => new()
    {
        Id        = Id,
        ProjectId = ProjectId,
        TeamId    = TeamId
    };
}
