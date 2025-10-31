using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.WebApi.Entities
{
    public class ToDoEntity
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public ToDoStatus Status { get; set; }
        public Guid? AssignedTo { get; set; }
    }
}
