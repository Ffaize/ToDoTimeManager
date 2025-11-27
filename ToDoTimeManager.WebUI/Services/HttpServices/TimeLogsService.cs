using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices
{
    public class TimeLogsService : BaseHttpService
    {
        private readonly ILogger<TimeLogsService> _logger;

        public TimeLogsService(IHttpClientFactory httpClientFactory, ILogger<TimeLogsService> logger) : base(
            httpClientFactory)
        {
            ApiControllerName = "TimeLogs";
            _logger = logger;
        }

        public async Task<List<TimeLog>> GetAllTimeLogs()
        {
            try
            {
                var response = await _httpClient.GetAsync(Url("GetAll"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<TimeLog>>();
                return result ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all TimeLogs");
                return [];
            }
        }

        public async Task<TimeLog?> GetTimeLogById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetById/{id}"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<TimeLog>();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TimeLog by ID: {TimeLogId}", id);
                return null;
            }
        }

        public async Task<List<TimeLog>> GetTimeLogsByToDoId(Guid toDoId)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetByToDoId/{toDoId}"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<TimeLog>>();
                return result ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TimeLogs for ToDo ID: {ToDoId}", toDoId);
                return [];
            }
        }

        public async Task<List<TimeLog>> GetTimeLogsByUserId(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetByUserId/{userId}"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<TimeLog>>();
                return result ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TimeLogs for User ID: {UserId}", userId);
                return [];
            }
        }

        public async Task<List<TimeLog>> GetTimeLogsByUserIdAndToDoId(Guid userId, Guid toDoId)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetByUserIdAndToDoId/{userId}/{toDoId}"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<TimeLog>>();
                return result ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TimeLogs for User ID: {UserId} and ToDo ID: {ToDoId}", userId,
                    toDoId);
                return [];
            }
        }

        public async Task<bool> CreateTimeLog(TimeLog timeLog)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(Url("Create"), timeLog);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TimeLog for ToDo ID: {ToDoId} and User ID: {UserId}",
                    timeLog.ToDoId, timeLog.UserId);
                return false;
            }
        }

        public async Task<bool> UpdateTimeLog(TimeLog timeLog)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(Url("Update"), timeLog);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TimeLog ID: {TimeLogId}", timeLog.Id);
                return false;
            }
        }

        public async Task<bool> DeleteTimeLog(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(Url($"Delete/{id}"));
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting TimeLog ID: {TimeLogId}", id);
                return false;
            }
        }
    }
}
