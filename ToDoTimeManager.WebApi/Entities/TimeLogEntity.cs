using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Entities;

public class TimeLogEntity
{

    public TimeLogEntity()
    {

    }


    public TimeLogEntity(TimeLog timeLog)
    {
        Id = timeLog.Id;
        ToDoId = timeLog.ToDoId;
        UserId = timeLog.UserId;
        HoursSpent = timeLog.HoursSpent;
        LogDate = timeLog.LogDate;
        LogDescription = timeLog.LogDescription;
    }

    public Guid Id { get; set; }
    public Guid ToDoId { get; set; }
    public Guid UserId { get; set; }
    public TimeOnly HoursSpent { get; set; }
    public DateTime LogDate { get; set; }
    public string? LogDescription { get; set; }

    public TimeLog? ToTimeLog()
    {
        return new TimeLog
        {
            Id = Id,
            ToDoId = ToDoId,
            UserId = UserId,
            HoursSpent = HoursSpent,
            LogDate = LogDate,
            LogDescription = LogDescription
        };
    }
}