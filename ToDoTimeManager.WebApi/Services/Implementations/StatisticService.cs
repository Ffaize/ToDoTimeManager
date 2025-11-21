using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class StatisticService : IStatisticService
{
    private readonly IToDosDataController _toDosDataController;
    public StatisticService(IToDosDataController toDosDataController)
    {
        _toDosDataController = toDosDataController;
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
}