using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class StatisticService : IStatisticService
{
    private readonly IToDosDataController _toDosDataController;
    private readonly ITimeLogsDataController _timeLogsDataController;
    public StatisticService(IToDosDataController toDosDataController,ITimeLogsDataController timeLogsDataController)
    {
        _toDosDataController = toDosDataController;
        _timeLogsDataController = timeLogsDataController;
    }

    public async Task<List<ToDoCountStatisticsOfAllTime>> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId)
    {
        var result = new List<ToDoCountStatisticsOfAllTime>();
        foreach (var status in Enum.GetValues<ToDoStatus>())
        {
            await GetCountOfStatusesByStatus(userId, status, result);
        }
        return result;
    }

    private async Task GetCountOfStatusesByStatus(Guid userId, ToDoStatus status, List<ToDoCountStatisticsOfAllTime> result)
    {
        var count = await _toDosDataController.GetToDosCountByUserIdAndStatus(userId, status);
        result.Add(new ToDoCountStatisticsOfAllTime
        {
            ToDoStatus = status,
            Count = count
        });
    }

    public async Task<MainPageStatisticModel?> GetMainPageStatistic(MainPageStatisticRequest filter)
    {
        var timeLogsForFilterTime = await _timeLogsDataController.GetTimeLogsByUserIdAndTime(filter.UserId, GetFilterDaysAgo(filter.TimeFilter));
        var timeLogsForThisMonth = await _timeLogsDataController.GetTimeLogsByUserIdAndTime(filter.UserId, DateTime.Now.Day);
        var toDosForNearestDueDate = await _toDosDataController.GetToDosByNearestDueDateByUserId(filter.UserId);
        var toDoCountStatisticsOfAllTimes = new List<ToDoCountStatisticsOfAllTime>();
        await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.New, toDoCountStatisticsOfAllTimes);
        await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.InProgress, toDoCountStatisticsOfAllTimes);
        await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.Completed, toDoCountStatisticsOfAllTimes);
        await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.Cancelled, toDoCountStatisticsOfAllTimes);

        return new MainPageStatisticModel()
        {
            TimeLogsForGivenTime = timeLogsForFilterTime.Select(x => x.ToTimeLog()).ToList(),
            TimeLogsForThisMonth = timeLogsForThisMonth.Select(x => x.ToTimeLog()).ToList(),
            DueDateTasks = toDosForNearestDueDate.ToDictionary(x => x.DueDate!.Value, x => x),
            ToDoStatuses = toDoCountStatisticsOfAllTimes
        };
    }

    private static int GetFilterDaysAgo(TimeFilter filterTimeFilter)
    {
        return filterTimeFilter switch
        {
            TimeFilter.DayAgo => 1,
            TimeFilter.WeekAgo => 7,
            TimeFilter.MonthAgo => 30,
            TimeFilter.YearAgo => 365,
            _ => -1
        };
    }
}