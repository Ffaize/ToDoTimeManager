using System.Globalization;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models;

public class ToDo
{
    public Guid Id { get; set; }
    public int NumberedId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }

    public string DisplayDueDate => DueDate != null
        ? ((DateTime)DueDate).ToLocalTime().ToShortDateString().ToString(CultureInfo.CurrentUICulture)
        : "-";

    public ToDoStatus Status { get; set; } = ToDoStatus.New;

    /// <summary>Gets or sets the work classification of this to-do item.</summary>
    public TaskType? Type { get; set; }

    public Guid? AssignedTo { get; set; }
    public Guid? TeamId { get; set; }
    public Guid? ProjectId { get; set; }
}