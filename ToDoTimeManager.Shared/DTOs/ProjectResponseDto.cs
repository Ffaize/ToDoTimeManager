using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Shared.DTOs;

public sealed class ProjectResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public int TeamCount { get; set; }
    public List<ProjectTeam>? Teams { get; set; }
}