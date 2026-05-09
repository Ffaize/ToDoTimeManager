using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Entities;

public class ActivityLogEntity
{
    public ActivityLogEntity()
    {
    }

    public ActivityLogEntity(ActivityLog activityLog)
    {
        Id           = activityLog.Id;
        ToDoId       = activityLog.ToDoId;
        UserId       = activityLog.UserId;
        Type         = activityLog.Type;
        Description  = activityLog.Description;
        ActivityTime = activityLog.ActivityTime;
    }

    public Guid         Id           { get; set; }
    public Guid?        ToDoId       { get; set; }
    public Guid         UserId       { get; set; }
    public ActivityType Type         { get; set; }
    public string       Description  { get; set; } = string.Empty;
    public DateTime     ActivityTime { get; set; }
    public string?      UserName       { get; set; }
    public string?      ToDoTitle      { get; set; }
    public int?         ToDoNumberedId { get; set; }

    public ActivityLog ToActivityLog()
    {
        return new ActivityLog
        {
            Id           = Id,
            ToDoId       = ToDoId,
            UserId       = UserId,
            Type         = Type,
            Description  = Description,
            ActivityTime = ActivityTime,
            UserName       = UserName,
            ToDoTitle      = ToDoTitle,
            ToDoNumberedId = ToDoNumberedId
        };
    }
}
