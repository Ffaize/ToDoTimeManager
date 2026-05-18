using Microsoft.Extensions.Configuration;
using ToDoTimeManager.Shared.DTOs.TwoFactorAuth;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebUI.Services.HttpServices;

public class AuthService : BaseHttpService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(IHttpClientFactory httpClientFactory, ILogger<AuthService> logger, IConfiguration configuration) : base(httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        ApiControllerName = "Auth";
    }

    public string GetGoogleLoginUrl()
    {
        var apiBase = _configuration["BaseApiUrlAddress"]?.TrimEnd('/') ?? string.Empty;
        return $"{apiBase}/api/Auth/GoogleLogin";
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
            _logger.LogError(ex, "Auth HTTP request failed");
            return null;
        }
    }

    public async Task<LoginResponse?> Login(LoginUser user)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("Login"), user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth HTTP request failed");
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
            _logger.LogError(ex, "Auth HTTP request failed");
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
            _logger.LogError(ex, "Auth HTTP request failed");
            return null;
        }
    }

    public async Task<TokenModel?> ExchangeGoogleSessionAsync(string code)
    {
        try
        {
            var response = await _httpClient.GetAsync(Url($"ExchangeGoogleSession?code={Uri.EscapeDataString(code)}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TokenModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth HTTP request failed");
            return null;
        }
    }
}
