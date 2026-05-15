using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Shared.DTOs.TwoFactorAuth;

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

    /// <summary>
    /// Initializes a new instance of <see cref="AuthController"/>.
    /// </summary>
    /// <param name="authService">The authentication service used to validate credentials.</param>
    /// <param name="twoFactorService">The two-factor authentication service used to manage verification codes and issue tokens.</param>
    public AuthController(IAuthService authService, ITwoFactorService twoFactorService, IUsersService usersService)
    {
        _authService = authService;
        _twoFactorService = twoFactorService;
        _usersService = usersService;
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
        var pending = await _authService.Login(loginUser!);
        return pending != null ? Ok(pending) : StatusCode(500);
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
}
