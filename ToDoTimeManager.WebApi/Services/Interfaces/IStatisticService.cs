using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IStatisticService
{
    Task<List<ToDoCountStatisticsOfAllTime>> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId, Guid currentUserId, bool isAdmin);
    Task<MainPageStatisticModel?> GetMainPageStatistic(MainPageStatisticRequestDto filter, Guid currentUserId, bool isAdmin);
}
