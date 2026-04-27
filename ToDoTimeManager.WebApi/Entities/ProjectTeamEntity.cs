using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Entities;

public class ProjectTeamEntity
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TeamId { get; set; }

    public ProjectTeam ToProjectTeam()
    {
        return new ProjectTeam
        {
            Id = Id,
            ProjectId = ProjectId,
            TeamId = TeamId
        };
    }
}