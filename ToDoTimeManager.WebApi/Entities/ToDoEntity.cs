using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Entities;

public class ToDoEntity
{

    public ToDoEntity()
    {

    }

    public ToDoEntity(ToDo toDo)
    {
        Id = toDo.Id;
        Title = toDo.Title;
        Description = toDo.Description;
        CreatedAt = toDo.CreatedAt;
        DueDate = toDo.DueDate;
        Status = toDo.Status;
        AssignedTo = toDo.AssignedTo;
        NumberedId = toDo.NumberedId;
    }

    public Guid Id { get; set; }
    public int NumberedId { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public ToDoStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }

    public ToDo ToToDo()
    {
        return new ToDo
        {
            Id = Id,
            Title = Title,
            Description = Description,
            CreatedAt = CreatedAt,
            DueDate = DueDate,
            Status = Status,
            AssignedTo = AssignedTo,
            NumberedId = NumberedId
        };
    }
}