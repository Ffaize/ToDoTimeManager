using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models;

public class TeamMember
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public TeamMemberRole Role { get; set; }
}