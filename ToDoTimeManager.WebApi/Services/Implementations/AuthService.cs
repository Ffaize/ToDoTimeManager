using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IPasswordHelperService _passwordHelperService;
    private readonly IJwtGeneratorService _jwtGeneratorService;
    private readonly IConfiguration _configuration;
    private readonly IUsersDataController _usersDataController;
    private readonly ITwoFactorService _twoFactorService;

    public AuthService(
        ILogger<AuthService> logger,
        IPasswordHelperService passwordHelperService,
        IJwtGeneratorService jwtGeneratorService,
        IConfiguration configuration,
        IUsersDataController usersDataController,
        ITwoFactorService twoFactorService)
    {
        _logger = logger;
        _passwordHelperService = passwordHelperService;
        _jwtGeneratorService = jwtGeneratorService;
        _configuration = configuration;
        _usersDataController = usersDataController;
        _twoFactorService = twoFactorService;
    }

    public async Task<TwoFactorPendingModel?> Login(LoginUser loginUser)
    {
        if (loginUser == null || string.IsNullOrWhiteSpace(loginUser.LoginParameter))
            throw new ValidationException("Login data is invalid");
        if (string.IsNullOrWhiteSpace(loginUser.Password))
            throw new ValidationException("Password is required");

        try
        {
            var userEntity = await _usersDataController.GetUserByLoginParameter(loginUser.LoginParameter);
            if (userEntity == null)
                throw new ValidationException("Invalid username or password");

            var user = userEntity.ToUser();
            var passwordHash = _passwordHelperService.HashPassword(user.Id.ToString(), loginUser.Password);
            if (!_passwordHelperService.VerifyPassword(user, passwordHash))
                throw new ValidationException("Invalid username or password");

            return await _twoFactorService.SendCode(user.Id);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public TokenModel? RefreshAuthToken(TokenModel tokenModel)
    {
        if (tokenModel == null || string.IsNullOrWhiteSpace(tokenModel.RefreshToken))
            throw new ValidationException("Token data is null or invalid");
        if (tokenModel.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new ValidationException("Refresh token has expired");

        try
        {
            var (userId, userRole) = ValidateAndReadToken(tokenModel.AccessToken!);
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userRole))
                throw new ValidationException("Could not refresh token");

            return new TokenModel
            {
                AccessToken = _jwtGeneratorService.GenerateAccessToken(userId, Enum.Parse<UserRole>(userRole)),
                RefreshToken = tokenModel.RefreshToken,
                RefreshTokenExpiresAt = tokenModel.RefreshTokenExpiresAt
            };
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    private (string? UserId, string? Role) ValidateAndReadToken(string token)
    {
        var key = _configuration["JwtSettings:Key"] ?? string.Empty;
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = false, // allow expired tokens to be refreshed
            ClockSkew = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParams, out var _);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = principal.FindFirstValue(ClaimTypes.Role);
        return (userId, role);
    }
}
