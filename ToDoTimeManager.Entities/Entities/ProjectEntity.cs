namespace ToDoTimeManager.Entities.Entities;

public class ProjectEntity
{
    public ProjectEntity()
    {
    }

    public ProjectEntity(Project project)
    {
        Id = project.Id;
        Name = project.Name;
        Description = project.Description;
        CreatedAt = project.CreatedAt;
        CreatedBy = project.CreatedBy;
        Type = project.Type;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public int TeamCount { get; set; }
    public ProjectType? Type { get; set; }

    public Project ToProject()
    {
        return new Project
        {
            Id = Id,
            Name = Name,
            Description = Description,
            CreatedAt = CreatedAt,
            CreatedBy = CreatedBy,
            TeamCount = TeamCount,
            Type = Type
        };
    }
}