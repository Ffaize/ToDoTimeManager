using System.ComponentModel.DataAnnotations;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs;

public sealed class ToDoUpsertRequestDto
{
    [NotEmptyGuid] public Guid Id { get; set; }

    // Kept for compatibility with existing UI/API payloads.
    public int NumberedId { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)] public string? Description { get; set; }

    [Required] public DateTime? CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    [Required] public ToDoStatus? Status { get; set; }

    /// <summary>The work classification. Optional; null means unclassified.</summary>
    public TaskType? Type { get; set; }

    public Guid? AssignedTo { get; set; }

    public Guid? TeamId { get; set; }

    public Guid? ProjectId { get; set; }
}