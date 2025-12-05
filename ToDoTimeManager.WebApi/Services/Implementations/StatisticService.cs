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
            var count = await _toDosDataController.GetToDosCountByUserIdAndStatus(userId, status);
            result.Add(new ToDoCountStatisticsOfAllTime
            {
                ToDoStatus = status,
                Count = count
            });
        }
        return result;
    }

    public async Task<MainPageStatisticModel?> GetMainPageStatistic(MainPageStatisticRequest filter)
    {
        var timeLogsForFilterTime = await _timeLogsDataController.GetTimeLogsByUserIdAndTime(filter.UserId, GetFilterDaysAgo(filter.TimeFilter));

        return new MainPageStatisticModel()
        {
            TimeLogsForGivenTime = timeLogsForFilterTime.Select(x => x.ToTimeLog()).ToList()
        };
    }

    private int GetFilterDaysAgo(TimeFilter filterTimeFilter)
    {
        return filterTimeFilter switch
        {
            TimeFilter.DayAgo => 1,
            TimeFilter.WeekAgo => 7,
            TimeFilter.MonthAgo => 30,
            TimeFilter.YearAgo => 365,
            _ => int.MaxValue
        };
    }
}