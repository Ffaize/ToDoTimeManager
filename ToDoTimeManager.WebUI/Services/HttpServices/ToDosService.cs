using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices
{
    public class ToDosService : BaseHttpService
    {
        private readonly ILogger<ToDosService> _logger;
        public ToDosService(IHttpClientFactory httpClientFactory, ILogger<ToDosService> logger) : base(httpClientFactory)
        {
            ApiControllerName = "ToDos";
            _logger = logger;
        }

        public async Task<List<ToDo>> GetAllToDos()
        {
            try
            {

                var response = await _httpClient.GetAsync(Url("GetAll"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<ToDo>>();
                return result ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all ToDos");
                return [];
            }
        }

        public async Task<ToDo?> GetToDoById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetById/{id}"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<ToDo>();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ToDo by ID: {ToDoId}", id);
                return null;
            }
        }

        public async Task<List<ToDo>> GetToDosByUserId(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetByUserId/{userId}"));
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<ToDo>>();
                return result ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ToDos for User ID: {UserId}", userId);
                return [];
            }
        }

        public async Task<bool> CreateToDo(ToDo toDo)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(Url("Create"), toDo);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ToDo");
                return false;
            }
        }

        public async Task<bool> UpdateToDo(ToDo toDo)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(Url("Update"), toDo);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ToDo");
                return false;
            }
        }

        public async Task<bool> DeleteToDo(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(Url($"Delete/{id}"));
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ToDo with ID: {ToDoId}", id);
                return false;
            }
        }

    }
}
