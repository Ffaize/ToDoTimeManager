using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Entities;

public class TeamEntity
{
    public TeamEntity() { }

    public TeamEntity(Team team)
    {
        Id          = team.Id;
        Name        = team.Name;
        Description = team.Description;
        CreatedAt   = team.CreatedAt;
        CreatedBy   = team.CreatedBy;
    }

    public Guid     Id          { get; set; }
    public string   Name        { get; set; } = string.Empty;
    public string?  Description { get; set; }
    public DateTime CreatedAt   { get; set; }
    public Guid     CreatedBy   { get; set; }
    public int      MemberCount { get; set; }

    public Team ToTeam() => new()
    {
        Id          = Id,
        Name        = Name,
        Description = Description,
        CreatedAt   = CreatedAt,
        CreatedBy   = CreatedBy,
        MemberCount = MemberCount
    };
}
