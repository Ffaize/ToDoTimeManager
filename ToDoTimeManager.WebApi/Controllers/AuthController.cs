using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Shared.DTOs.TwoFactorAuth;
using ToDoTimeManager.Shared.DTOs.PasswordReset;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Handles authentication operations including login, two-factor code management, and token refresh.
/// All endpoints are publicly accessible without a prior authentication token.
/// </summary>
[AllowAnonymous]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IUsersService _usersService;
    private readonly IPasswordResetService _passwordResetService;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public AuthController(
        IAuthService authService,
        ITwoFactorService twoFactorService,
        IUsersService usersService,
        IPasswordResetService passwordResetService,
        IMemoryCache cache,
        IConfiguration configuration)
    {
        _authService = authService;
        _twoFactorService = twoFactorService;
        _usersService = usersService;
        _passwordResetService = passwordResetService;
        _cache = cache;
        _configuration = configuration;
    }

    /// <summary>
    /// Validates the user's credentials and sends a two-factor verification code to their registered email address.
    /// The JWT token is <b>not</b> returned at this step — the client must call <c>VerifyCode</c> to complete authentication.
    /// </summary>
    /// <param name="loginUser">The login credentials containing a username or email and a password.</param>
    /// <returns>
    /// 200 OK with a <see cref="TwoFactorPendingModel"/> containing the user ID and masked email address on success;
    /// 500 Internal Server Error if authentication fails unexpectedly.
    /// </returns>
    [HttpPost("Login")]
    [EnableRateLimiting("auth-login")]
    public async Task<IActionResult> Login(LoginUser? loginUser)
    {
        var response = await _authService.Login(loginUser!);
        return response != null ? Ok(response) : StatusCode(500);
    }

    /// <summary>
    /// Generates a new two-factor verification code and sends it to the user's registered email address.
    /// Use this endpoint to resend a code when the previous one has expired or was not received.
    /// </summary>
    /// <param name="request">The request containing the user ID for which to send the code.</param>
    /// <returns>
    /// 200 OK with a <see cref="TwoFactorPendingModel"/> containing the user ID and masked email address.
    /// </returns>
    [HttpPost("SendCode")]
    [EnableRateLimiting("auth-send-code")]
    public async Task<IActionResult> SendCode([FromBody] SendTwoFactorCodeRequestDto request)
    {
        var user = await _usersService.GetUserById(request.UserId, GetCurrentUserId(), GetCurrentUserRole());
        var pending = await _twoFactorService.SendCode(new UserEntity(user));
        return Ok(pending);
    }

    /// <summary>
    /// Verifies the two-factor code submitted by the user and, upon success, issues a JWT access token
    /// paired with a refresh token. The code is invalidated immediately after use.
    /// </summary>
    /// <param name="request">The request containing the user ID and the verification code.</param>
    /// <returns>
    /// 200 OK with a <see cref="TokenModel"/> containing the access token, refresh token, and refresh token expiry on success;
    /// 400 Bad Request if the code is invalid or has expired.
    /// </returns>
    [HttpPost("VerifyCode")]
    [EnableRateLimiting("auth-verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyTwoFactorRequestDto request)
    {
        var tokenModel = await _twoFactorService.VerifyCode(request.UserId, request.Code!, request.KeepSignedIn);
        return Ok(tokenModel);
    }

    /// <summary>
    /// Issues a new access token using a valid, non-expired refresh token.
    /// The refresh token itself is not rotated — the same refresh token and its expiry are preserved.
    /// </summary>
    /// <param name="tokenModel">The current token pair containing the refresh token to exchange.</param>
    /// <returns>
    /// 200 OK with a refreshed <see cref="TokenModel"/> on success;
    /// 500 Internal Server Error if the refresh token is invalid or expired.
    /// </returns>
    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken(TokenModel? tokenModel)
    {
        var newTokenModel = await _authService.RefreshAuthToken(tokenModel!);
        return newTokenModel != null ? Ok(newTokenModel) : StatusCode(500);
    }

    [HttpPost("ForgotPassword")]
    [EnableRateLimiting("auth-forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var pending = await _passwordResetService.SendResetCode(request.Email);
        return Ok(pending);
    }

    [HttpPost("ResetPassword")]
    [EnableRateLimiting("auth-reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        await _passwordResetService.ResetPassword(request.UserId, request.Code, request.NewPassword);
        return Ok();
    }

    [HttpGet("GoogleLogin")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("GoogleCallback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync("ExternalCookieScheme");
        if (!result.Succeeded || result.Principal == null)
            return Redirect(BuildWebUIUrl("/auth"));

        var oauthResult = await _authService.GetOrCreateGoogleUserTokenAsync(result.Principal);

        await HttpContext.SignOutAsync("ExternalCookieScheme");

        if (oauthResult == null)
            return Redirect(BuildWebUIUrl("/auth"));

        var code = Guid.NewGuid().ToString("N");
        _cache.Set($"google:{code}", oauthResult, TimeSpan.FromSeconds(30));

        return Redirect(BuildWebUIUrl($"/auth?google_session={code}"));
    }

    [HttpGet("ExchangeGoogleSession")]
    public IActionResult ExchangeGoogleSession([FromQuery] string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest();

        if (!_cache.TryGetValue($"google:{code}", out OAuthExchangeResult? oauthResult) || oauthResult == null)
            return NotFound();

        _cache.Remove($"google:{code}");
        return Ok(oauthResult);
    }

    [HttpGet("GitHubLogin")]
    public IActionResult GitHubLogin()
    {
        var redirectUrl = Url.Action(nameof(GitHubCallback), "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GitHubAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("GitHubCallback")]
    public async Task<IActionResult> GitHubCallback()
    {
        var result = await HttpContext.AuthenticateAsync("ExternalCookieScheme");
        if (!result.Succeeded || result.Principal == null)
            return Redirect(BuildWebUIUrl("/auth"));

        var oauthResult = await _authService.GetOrCreateGitHubUserTokenAsync(result.Principal);

        await HttpContext.SignOutAsync("ExternalCookieScheme");

        if (oauthResult == null)
            return Redirect(BuildWebUIUrl("/auth"));

        var code = Guid.NewGuid().ToString("N");
        _cache.Set($"github:{code}", oauthResult, TimeSpan.FromSeconds(30));

        return Redirect(BuildWebUIUrl($"/auth?github_session={code}"));
    }

    [HttpGet("ExchangeGitHubSession")]
    public IActionResult ExchangeGitHubSession([FromQuery] string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest();

        if (!_cache.TryGetValue($"github:{code}", out OAuthExchangeResult? oauthResult) || oauthResult == null)
            return NotFound();

        _cache.Remove($"github:{code}");
        return Ok(oauthResult);
    }

    private string BuildWebUIUrl(string path)
    {
        var baseUrl = _configuration["WebUIBaseUrl"]?.TrimEnd('/') ?? string.Empty;
        return $"{baseUrl}{path}";
    }
}
