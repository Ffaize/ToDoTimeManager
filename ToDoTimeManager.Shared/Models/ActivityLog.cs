using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Shared.Models;

public class ActivityLog
{
    public Guid         Id           { get; set; }
    public Guid?        ToDoId       { get; set; }
    public Guid         UserId       { get; set; }
    public ActivityType Type         { get; set; }
    public string       Description  { get; set; } = string.Empty;
    public DateTime     ActivityTime { get; set; }
    public string?      UserName       { get; set; }
    public string?      ToDoTitle      { get; set; }
    public int?         ToDoNumberedId { get; set; }
}
