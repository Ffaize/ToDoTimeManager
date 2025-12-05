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

        public async Task<MainPageStatisticModel> GetMainPageStatistic(MainPageStatisticRequest filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(Url("GetMainPageStatistic"), filter);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<MainPageStatisticModel>();
                return result ?? new MainPageStatisticModel();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while fetching main page statistics for user {UserId} with filter {TimeFilter}", filter.UserId, filter.TimeFilter);
                return new MainPageStatisticModel();
            }
        }
    }
}
