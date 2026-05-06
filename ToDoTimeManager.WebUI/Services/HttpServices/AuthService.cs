using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices;

public class AuthService : BaseHttpService
{
    private readonly ILogger<AuthService> _logger;

    public AuthService(IHttpClientFactory httpClientFactory, ILogger<AuthService> logger) : base(httpClientFactory)
    {
        _logger = logger;
        ApiControllerName = "Auth";
    }

    public async Task<TokenModel?> RefreshToken(TokenModel tokens, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("RefreshToken"), tokens, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TokenModel>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return null;
        }
    }

    // Returns TwoFactorPendingModel — the JWT is issued only after VerifyCode succeeds.
    public async Task<TwoFactorPendingModel?> Login(LoginUser user)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("Login"), user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TwoFactorPendingModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return null;
        }
    }

    public async Task<TwoFactorPendingModel?> SendCode(SendTwoFactorCodeRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("SendCode"), request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TwoFactorPendingModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return null;
        }
    }

    public async Task<TokenModel?> VerifyCode(VerifyTwoFactorRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("VerifyCode"), request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TokenModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return null;
        }
    }
}
