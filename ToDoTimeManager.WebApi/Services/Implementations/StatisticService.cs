using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class StatisticService : IStatisticService
{
    private readonly IToDosDataController _toDosDataController;
    private readonly ITimeLogsDataController _timeLogsDataController;
    private readonly ILogger<StatisticService> _logger;

    public StatisticService(
        IToDosDataController toDosDataController,
        ITimeLogsDataController timeLogsDataController,
        ILogger<StatisticService> logger)
    {
        _toDosDataController = toDosDataController;
        _timeLogsDataController = timeLogsDataController;
        _logger = logger;
    }

    public async Task<List<ToDoCountStatisticsOfAllTime>> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId, Guid currentUserId, bool isAdmin)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (userId != currentUserId && !isAdmin)
            throw new ForbiddenException();

        var result = new List<ToDoCountStatisticsOfAllTime>();
        foreach (var status in Enum.GetValues<ToDoStatus>())
        {
            await GetCountOfStatusesByStatus(userId, status, result);
        }
        return result;
    }

    public async Task<MainPageStatisticModel?> GetMainPageStatistic(MainPageStatisticRequest filter, Guid currentUserId, bool isAdmin)
    {
        if (filter.UserId == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (filter.UserId != currentUserId && !isAdmin)
            throw new ForbiddenException();

        try
        {
            var timeLogsForFilterTime = await _timeLogsDataController.GetTimeLogsByUserIdAndTime(filter.UserId, GetFilterDaysAgo(filter.TimeFilter));
            var daysIntoCurrentMonth = (int)(DateTime.UtcNow - new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc)).TotalDays;
            var timeLogsForThisMonth = await _timeLogsDataController.GetTimeLogsByUserIdAndTime(filter.UserId, daysIntoCurrentMonth);
            var toDosForNearestDueDate = await _toDosDataController.GetToDosByNearestDueDateByUserId(filter.UserId);
            var toDoCountStatisticsOfAllTimes = new List<ToDoCountStatisticsOfAllTime>();
            await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.New, toDoCountStatisticsOfAllTimes);
            await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.InProgress, toDoCountStatisticsOfAllTimes);
            await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.Completed, toDoCountStatisticsOfAllTimes);
            await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.Cancelled, toDoCountStatisticsOfAllTimes);

            return new MainPageStatisticModel
            {
                TimeLogsForGivenTime = timeLogsForFilterTime.Select(x => x.ToTimeLog()).ToList(),
                TimeLogsForThisMonth = timeLogsForThisMonth.Select(x => x.ToTimeLog()).ToList(),
                DueDateTasks         = toDosForNearestDueDate.ToDictionary(x => x.DueDate!.Value, x => x),
                ToDoStatuses         = toDoCountStatisticsOfAllTimes
            };
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    private async Task GetCountOfStatusesByStatus(Guid userId, ToDoStatus status, List<ToDoCountStatisticsOfAllTime> result)
    {
        var count = await _toDosDataController.GetToDosCountByUserIdAndStatus(userId, status);
        result.Add(new ToDoCountStatisticsOfAllTime
        {
            ToDoStatus = status,
            Count      = count
        });
    }

    private static int GetFilterDaysAgo(TimeFilter filterTimeFilter)
    {
        return filterTimeFilter switch
        {
            TimeFilter.DayAgo   => 1,
            TimeFilter.WeekAgo  => 7,
            TimeFilter.MonthAgo => 30,
            TimeFilter.YearAgo  => 365,
            _                   => -1
        };
    }
}
