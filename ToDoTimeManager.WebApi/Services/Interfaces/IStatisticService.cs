using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IStatisticService
{
    Task<List<ToDoCountStatisticsOfAllTime>> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId);
}