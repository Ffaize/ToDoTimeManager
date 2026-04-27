using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Entities;

public class TeamMemberEntity
{
    public TeamMemberEntity()
    {
    }

    public TeamMemberEntity(TeamMember teamMember)
    {
        Id = teamMember.Id;
        TeamId = teamMember.TeamId;
        UserId = teamMember.UserId;
        Role = teamMember.Role;
    }

    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public TeamMemberRole Role { get; set; }

    public TeamMember ToTeamMember()
    {
        return new TeamMember
        {
            Id = Id,
            TeamId = TeamId,
            UserId = UserId,
            Role = Role
        };
    }
}