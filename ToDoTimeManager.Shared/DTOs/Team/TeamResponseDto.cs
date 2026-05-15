using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Shared.DTOs.Team;

public sealed class TeamResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public int MemberCount { get; set; }
    public List<TeamMember>? Members { get; set; }
}