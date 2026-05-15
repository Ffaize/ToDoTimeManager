using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Interfaces;

public interface IStatisticService
{
    Task<List<ToDoCountStatisticsOfAllTime>> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId, Guid currentUserId,
        UserRole currentUserRole);

    Task<MainPageStatisticModel?> GetMainPageStatistic(MainPageStatisticRequestDto filter, Guid currentUserId,
        UserRole currentUserRole);
}
