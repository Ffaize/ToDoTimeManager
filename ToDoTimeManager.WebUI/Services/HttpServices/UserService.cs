using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices
{
    public class UserService : BaseHttpService
    {
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger) : base(httpClientFactory)
        {
            _logger = logger;
            ApiControllerName = "User";
        }

        public async Task<List<User>?> GetAllUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync(Url("GetAll"));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return null;
            }
        }

        public async Task<User?> GetUserById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetById/{id}"));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return null;
            }
        }

        public async Task<User?> GetUserByUsername(string userName)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetByUsername/{userName}"));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return null;
            }
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetByEmail/{email}"));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return null;
            }
        }

        public async Task<User?> GetUserByLoginParameter(string loginParameter)
        {
            try
            {
                var response = await _httpClient.GetAsync(Url($"GetByLoginParameter/{loginParameter}"));
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return null;
            }
        }

        public async Task<bool> Create(User user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(Url("Create"), user);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return false;
            }
        }
        public async Task<bool> Update(User user)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(Url("Update"), user);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(Url($"Delete/{id}"));
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return false;
            }
        }

    }
}
