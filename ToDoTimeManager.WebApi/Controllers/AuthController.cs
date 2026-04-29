using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Handles authentication operations including login, 2FA code verification, and token refresh.
/// All endpoints are publicly accessible without a prior authentication token.
/// </summary>
[AllowAnonymous]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly ITwoFactorService _twoFactorService;

    public AuthController(IAuthService authService, ITwoFactorService twoFactorService)
    {
        _authService = authService;
        _twoFactorService = twoFactorService;
    }

    /// <summary>
    /// Validates credentials and sends a 2FA code to the user's email.
    /// The token is NOT returned here — call VerifyCode after entering the code.
    /// </summary>
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUser? loginUser)
    {
        var pending = await _authService.Login(loginUser!);
        return pending != null ? Ok(pending) : StatusCode(500);
    }

    /// <summary>
    /// Resends a 2FA code to the user's email (e.g. when the previous one expired).
    /// </summary>
    [HttpPost("SendCode")]
    public async Task<IActionResult> SendCode([FromBody] SendTwoFactorCodeRequestDto request)
    {
        var pending = await _twoFactorService.SendCode(request.UserId);
        return Ok(pending);
    }

    /// <summary>
    /// Verifies the 2FA code and returns a JWT access token + refresh token on success.
    /// </summary>
    [HttpPost("VerifyCode")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyTwoFactorRequestDto request)
    {
        var tokenModel = await _twoFactorService.VerifyCode(request.UserId, request.Code!);
        return Ok(tokenModel);
    }

    /// <summary>
    /// Issues a new access token using a valid, non-expired refresh token.
    /// </summary>
    [HttpPost("RefreshToken")]
    public IActionResult RefreshToken(TokenModel? tokenModel)
    {
        var newTokenModel = _authService.RefreshAuthToken(tokenModel!);
        return newTokenModel != null ? Ok(newTokenModel) : StatusCode(500);
    }
}
