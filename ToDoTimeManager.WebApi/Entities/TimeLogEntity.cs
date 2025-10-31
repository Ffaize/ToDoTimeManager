namespace ToDoTimeManager.WebApi.Entities
{
    public class TimeLogEntity
    {
        public Guid Id { get; set; }
        public Guid ToDoId { get; set; }
        public Guid UserId { get; set; }
        public TimeOnly HoursSpent { get; set; }
        public DateTime LogDate { get; set; }
        public string? LogDescription { get; set; }
    }
}
