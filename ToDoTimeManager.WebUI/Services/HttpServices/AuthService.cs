using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices
{
    public class AuthService : BaseHttpService
    {
        private readonly ILogger<AuthService> _logger;
        public AuthService(IHttpClientFactory httpClientFactory, ILogger<AuthService> logger) : base(httpClientFactory)
        {
            _logger = logger;
            ApiControllerName = "Auth";
        }

        public async Task<TokenModel?> RefreshToken(TokenModel tokens)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(Url("RefreshToken"), tokens);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TokenModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return null;
            }
        }

        public async Task<TokenModel?> Login(LoginUser user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(Url("Login"), user);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TokenModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message, ex);
                return null;
            }
        }

    }
}
