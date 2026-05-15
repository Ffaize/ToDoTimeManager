using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Entities.Exceptions;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IPasswordHelperService _passwordHelperService;
    private readonly IJwtGeneratorService _jwtGeneratorService;
    private readonly IConfiguration _configuration;
    private readonly IUsersDataController _usersDataController;
    private readonly IUserSecretsDataController _userSecretsDataController;
    private readonly ITwoFactorService _twoFactorService;

    public AuthService(
        ILogger<AuthService> logger,
        IPasswordHelperService passwordHelperService,
        IJwtGeneratorService jwtGeneratorService,
        IConfiguration configuration,
        IUsersDataController usersDataController,
        IUserSecretsDataController userSecretsDataController,
        ITwoFactorService twoFactorService)
    {
        _logger = logger;
        _passwordHelperService = passwordHelperService;
        _jwtGeneratorService = jwtGeneratorService;
        _configuration = configuration;
        _usersDataController = usersDataController;
        _userSecretsDataController = userSecretsDataController;
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

            var passwordSalt = await _userSecretsDataController.GetPasswordSaltByUserId(userEntity.Id);
            if (passwordSalt == null || !_passwordHelperService.VerifyPassword(loginUser.Password, userEntity.Password!, passwordSalt))
                throw new ValidationException("Invalid username or password");

            return await _twoFactorService.SendCode(userEntity);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth operation failed");
            return null;
        }
    }

    public async Task<TokenModel?> RefreshAuthToken(TokenModel? tokenModel)
    {
        if (tokenModel == null || string.IsNullOrWhiteSpace(tokenModel.RefreshToken)
                               || string.IsNullOrWhiteSpace(tokenModel.AccessToken))
            return null;

        try
        {
            var (userId, userRole) = ValidateAndReadToken(tokenModel.AccessToken);
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userRole))
                return null;

            var secrets = await _userSecretsDataController.GetByUserId(Guid.Parse(userId));
            if (secrets?.RefreshToken == null || secrets.RefreshTokenExpiresAt == null
                                               || secrets.RefreshTokenExpiresAt < DateTime.UtcNow)
                return null;

            var incomingHash = HashHelper.HashRefreshToken(tokenModel.RefreshToken);
            var storedBytes = Convert.FromBase64String(secrets.RefreshToken);
            var incomingBytes = Convert.FromBase64String(incomingHash);
            if (!CryptographicOperations.FixedTimeEquals(storedBytes, incomingBytes))
                return null;

            var newRefreshToken = _jwtGeneratorService.GenerateRefreshToken();
            await _userSecretsDataController.UpdateRefreshToken(
                Guid.Parse(userId), HashHelper.HashRefreshToken(newRefreshToken), secrets.RefreshTokenExpiresAt);

            return new TokenModel
            {
                AccessToken = _jwtGeneratorService.GenerateAccessToken(userId, Enum.Parse<UserRole>(userRole)),
                RefreshToken = newRefreshToken,
                RefreshTokenExpiresAt = secrets.RefreshTokenExpiresAt
            };
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth operation failed");
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
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParams, out var _);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = principal.FindFirstValue(ClaimTypes.Role);
        return (userId, role);
    }
}
