using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Handles authentication operations including login and token refresh.
/// All endpoints are publicly accessible without a prior authentication token.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of <see cref="AuthController"/>.
    /// </summary>
    /// <param name="authService">The authentication service used to validate credentials and issue tokens.</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user with their credentials and returns a JWT access token paired with a refresh token.
    /// </summary>
    /// <param name="loginUser">The login credentials containing a username or email and a password.</param>
    /// <returns>
    /// 200 OK with a <see cref="TokenModel"/> containing the access and refresh tokens on success;
    /// 500 Internal Server Error if authentication fails.
    /// </returns>
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUser? loginUser)
    {
        var tokenModel = await _authService.Login(loginUser!);
        return tokenModel != null ? Ok(tokenModel) : StatusCode(500);
    }

    /// <summary>
    /// Issues a new access token using a valid, non-expired refresh token.
    /// </summary>
    /// <param name="tokenModel">The current token pair containing the refresh token to exchange.</param>
    /// <returns>
    /// 200 OK with a refreshed <see cref="TokenModel"/> on success;
    /// 500 Internal Server Error if the refresh token is invalid or expired.
    /// </returns>
    [HttpPost("RefreshToken")]
    public IActionResult RefreshToken(TokenModel? tokenModel)
    {
        var newTokenModel = _authService.RefreshAuthToken(tokenModel!);
        return newTokenModel != null ? Ok(newTokenModel) : StatusCode(500);
    }
}