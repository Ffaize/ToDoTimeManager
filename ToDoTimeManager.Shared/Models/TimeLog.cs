namespace ToDoTimeManager.Shared.Models
{
    public class TimeLog
    {
        public Guid Id { get; set; }
        public Guid ToDoId { get; set; }
        public Guid UserId { get; set; }
        public TimeOnly HoursSpent { get; set; }
        public DateTime LogDate { get; set; }
        public string? LogDescription { get; set; }
    }
}
