using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IUsersService _usersService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, IUsersService usersService)
        {
            _logger = logger;
            _authService = authService;
            _usersService = usersService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser? loginUser)
        {
            if(loginUser is null)
                return Unauthorized("Login data is null");
            if(string.IsNullOrWhiteSpace(loginUser.Password) || string.IsNullOrWhiteSpace(loginUser.LoginParameter))
                return Unauthorized("Login data is invalid");

            var user = await _usersService.GetUserByLoginParameter(loginUser.LoginParameter);
            if (user is null)
                return Unauthorized("User was not found");

            var tokenModel = _authService.AuthenticateUser(loginUser, user);
            if (tokenModel is null)
                return Unauthorized("Invalid username or password");
            return Ok(tokenModel);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenModel? tokenModel)
        {
            if (tokenModel is null || string.IsNullOrWhiteSpace(tokenModel.RefreshToken))
                return Unauthorized("Token data is null or invalid");

            if(tokenModel.RefreshTokenExpiresAt > DateTime.UtcNow)
                return Unauthorized("Refresh token has not expired yet");

            var newTokenModel = _authService.RefreshAuthToken(tokenModel);
            if (newTokenModel is null)
                return Unauthorized("Could not refresh token");
            return Ok(newTokenModel);
        }
    }
}
