namespace ToDoTimeManager.Shared.Models;

public class Team
{
    public Guid    Id          { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt  { get; set; }
    public Guid    CreatedBy   { get; set; }
    public int     MemberCount { get; set; }
}
