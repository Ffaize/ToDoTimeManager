using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Entities.Exceptions;

namespace ToDoTimeManager.Business.Services.Implementations;

public class StatisticService : IStatisticService
{
    private readonly IToDosService _toDosService;
    private readonly ITimeLogsService _timeLogsService;
    private readonly ILogger<StatisticService> _logger;

    public StatisticService(
        IToDosService toDosService,
        ITimeLogsService timeLogsService,
        ILogger<StatisticService> logger)
    {
        _toDosService = toDosService;
        _timeLogsService = timeLogsService;
        _logger = logger;
    }

    public async Task<List<ToDoCountStatisticsOfAllTime>> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId,
        Guid currentUserId, UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (userId != currentUserId && currentUserRole < UserRole.Admin)
            throw new ForbiddenException();

        var result = new List<ToDoCountStatisticsOfAllTime>();
        foreach (var status in Enum.GetValues<ToDoStatus>()) 
            await GetCountOfStatusesByStatus(userId, status, result);
        return result;
    }

    public async Task<MainPageStatisticModel?> GetMainPageStatistic(MainPageStatisticRequestDto filter,
        Guid currentUserId, UserRole currentUserRole)
    {
        if (filter.UserId == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (filter.UserId != currentUserId && currentUserRole < UserRole.Admin)
            throw new ForbiddenException();

        try
        {
            List<TimeLog> timeLogsForFilterTime =
                await _timeLogsService.GetTimeLogsByUserIdAndTime(filter.UserId, filter.TimeFilter.ToDaysAgo());
            var daysIntoCurrentMonth =
                (int)(DateTime.UtcNow - new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0,
                    DateTimeKind.Utc)).TotalDays;
            List<TimeLog> timeLogsForThisMonth =
                await _timeLogsService.GetTimeLogsByUserIdAndTime(filter.UserId, daysIntoCurrentMonth);
            List<ToDo> toDosForNearestDueDate = await _toDosService.GetToDosByNearestDueDateByUserId(filter.UserId);
            var toDoCountStatisticsOfAllTimes = new List<ToDoCountStatisticsOfAllTime>();
            await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.New, toDoCountStatisticsOfAllTimes);
            await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.InProgress, toDoCountStatisticsOfAllTimes);
            await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.Completed, toDoCountStatisticsOfAllTimes);
            await GetCountOfStatusesByStatus(filter.UserId, ToDoStatus.Cancelled, toDoCountStatisticsOfAllTimes);

            return new MainPageStatisticModel
            {
                TimeLogsForGivenTime = timeLogsForFilterTime,
                TimeLogsForThisMonth = timeLogsForThisMonth,
                DueDateTasks = toDosForNearestDueDate.ToDictionary(x => x.DueDate!.Value, x => x),
                ToDoStatuses = toDoCountStatisticsOfAllTimes
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

    private async Task GetCountOfStatusesByStatus(Guid userId, ToDoStatus status,
        List<ToDoCountStatisticsOfAllTime> result)
    {
        var count = await _toDosService.GetToDosCountByUserIdAndStatus(userId, status);
        result.Add(new ToDoCountStatisticsOfAllTime
        {
            ToDoStatus = status,
            Count = count
        });
    }

}
