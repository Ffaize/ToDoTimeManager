using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices
{
    public class StatisticService : BaseHttpService
    {
        private readonly ILogger<StatisticService> _logger;
        public StatisticService(IHttpClientFactory httpClientFactory, ILogger<StatisticService> logger) : base(httpClientFactory)
        {
            ApiControllerName = "Statistic";
            _logger = logger;
        }

        public async Task<List<ToDoCountStatisticsOfAllTime>> GetToDoCountStatisticsOfAllTimeByUserId(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetToDoCountStatisticsOfAllTimeByUserId/{userId}"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<ToDoCountStatisticsOfAllTime>>();
                return result ?? [];
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while fetching ToDo count statistics for user {UserId}", userId);
                return [];
            }
        }
    }
}
