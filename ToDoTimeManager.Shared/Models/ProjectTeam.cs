namespace ToDoTimeManager.Shared.Models;

public class ProjectTeam
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TeamId { get; set; }
    public Guid? ProjectManagerId { get; set; }
}