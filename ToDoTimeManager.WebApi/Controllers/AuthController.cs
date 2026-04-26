using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUser? loginUser)
    {
        var tokenModel = await _authService.Login(loginUser!);
        return tokenModel != null ? Ok(tokenModel) : StatusCode(500);
    }

    [HttpPost("RefreshToken")]
    public IActionResult RefreshToken(TokenModel? tokenModel)
    {
        var newTokenModel = _authService.RefreshAuthToken(tokenModel!);
        return newTokenModel != null ? Ok(newTokenModel) : StatusCode(500);
    }
}
