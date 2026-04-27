using System.ComponentModel.DataAnnotations;
using ToDoTimeManager.Shared.Utils;

namespace ToDoTimeManager.Shared.DTOs;

public sealed class TimeLogUpsertRequestDto
{
    [NotEmptyGuid] public Guid Id { get; set; }

    [NotEmptyGuid] public Guid ToDoId { get; set; }

    [NotEmptyGuid] public Guid UserId { get; set; }

    [Required] public TimeSpan? HoursSpent { get; set; }

    [Required] public DateTime? LogDate { get; set; }

    [MaxLength(1000)] public string? LogDescription { get; set; }
}