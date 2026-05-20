using Microsoft.Extensions.Configuration;
using ToDoTimeManager.Shared.DTOs.TwoFactorAuth;
using ToDoTimeManager.Shared.DTOs.PasswordReset;
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

    public string GetGitHubLoginUrl()
    {
        var apiBase = _configuration["BaseApiUrlAddress"]?.TrimEnd('/') ?? string.Empty;
        return $"{apiBase}/api/Auth/GitHubLogin";
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

    public async Task<PasswordResetPendingModel?> ForgotPassword(string email)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("ForgotPassword"), new ForgotPasswordRequestDto { Email = email });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PasswordResetPendingModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth HTTP request failed");
            return null;
        }
    }

    public async Task<bool> ResetPassword(Guid userId, string code, string newPassword)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(Url("ResetPassword"), new ResetPasswordRequestDto
            {
                UserId = userId,
                Code = code,
                NewPassword = newPassword
            });
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth HTTP request failed");
            return false;
        }
    }

    public async Task<OAuthExchangeResult?> ExchangeGoogleSessionAsync(string code)
    {
        try
        {
            var response = await _httpClient.GetAsync(Url($"ExchangeGoogleSession?code={Uri.EscapeDataString(code)}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OAuthExchangeResult>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth HTTP request failed");
            return null;
        }
    }

    public async Task<OAuthExchangeResult?> ExchangeGitHubSessionAsync(string code)
    {
        try
        {
            var response = await _httpClient.GetAsync(Url($"ExchangeGitHubSession?code={Uri.EscapeDataString(code)}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OAuthExchangeResult>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth HTTP request failed");
            return null;
        }
    }
}
