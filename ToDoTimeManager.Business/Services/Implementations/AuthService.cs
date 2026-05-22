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
    private readonly IUserSettingsDataController _userSettingsDataController;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IUsersService _usersService;

    public AuthService(
        ILogger<AuthService> logger,
        IPasswordHelperService passwordHelperService,
        IJwtGeneratorService jwtGeneratorService,
        IConfiguration configuration,
        IUsersDataController usersDataController,
        IUserSecretsDataController userSecretsDataController,
        IUserSettingsDataController userSettingsDataController,
        ITwoFactorService twoFactorService,
        IUsersService usersService)
    {
        _logger = logger;
        _passwordHelperService = passwordHelperService;
        _jwtGeneratorService = jwtGeneratorService;
        _configuration = configuration;
        _usersDataController = usersDataController;
        _userSecretsDataController = userSecretsDataController;
        _userSettingsDataController = userSettingsDataController;
        _twoFactorService = twoFactorService;
        _usersService = usersService;
    }

    public async Task<LoginResponse?> Login(LoginUser loginUser)
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

            var isTwoFactorEnabled = await _userSettingsDataController.GetTwoFactorEnabled(userEntity.Id);
            if (isTwoFactorEnabled == true)
            {
                var pending = await _twoFactorService.SendCode(userEntity);
                return new LoginResponse { RequiresTwoFactor = true, TwoFaPending = pending };
            }

            var token = await GenerateTokenForUser(userEntity.Id, userEntity.UserRole!.Value, loginUser.KeepSignedIn);
            return new LoginResponse { RequiresTwoFactor = false, Token = token };
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

    private async Task<TokenModel> GenerateTokenForUser(Guid userId, UserRole userRole, bool keepSignedIn)
    {
        string? plainRefreshToken = null;
        DateTime? refreshExpiresAt = null;

        if (keepSignedIn)
        {
            await EnsureUserSecretsRowAsync(userId);

            var rtDays = int.TryParse(_configuration["JwtSettings:RefreshTokenLifetime"], out var days) ? days : 14;
            plainRefreshToken = _jwtGeneratorService.GenerateRefreshToken();
            refreshExpiresAt = DateTime.UtcNow.AddDays(rtDays);
            await _userSecretsDataController.UpdateRefreshToken(
                userId, HashHelper.HashRefreshToken(plainRefreshToken), refreshExpiresAt);
        }
        else
        {
            await _userSecretsDataController.ClearRefreshToken(userId);
        }

        return new TokenModel
        {
            AccessToken = _jwtGeneratorService.GenerateAccessToken(userId.ToString(), userRole),
            RefreshToken = plainRefreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }

    private async Task EnsureUserSecretsRowAsync(Guid userId)
    {
        var existing = await _userSecretsDataController.GetByUserId(userId);
        if (existing != null) return;

        var salt = _passwordHelperService.GenerateSalt();
        await _userSecretsDataController.Create(new UserSecretsEntity
        {
            Id           = Guid.NewGuid(),
            UserId       = userId,
            PasswordSalt = salt
        });
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

    public async Task<OAuthExchangeResult?> GetOrCreateGoogleUserTokenAsync(ClaimsPrincipal googleUser)
        => await HandleOAuthLoginAsync(googleUser, isGoogle: true);

    public async Task<OAuthExchangeResult?> GetOrCreateGitHubUserTokenAsync(ClaimsPrincipal githubUser)
        => await HandleOAuthLoginAsync(githubUser, isGoogle: false);

    private async Task<OAuthExchangeResult?> HandleOAuthLoginAsync(ClaimsPrincipal principal, bool isGoogle)
    {
        var providerName = isGoogle ? "Google" : "GitHub";
        try
        {
            var email = principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var displayName = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

            var avatarUrl = isGoogle
                ? (principal.FindFirstValue("picture") ?? principal.FindFirstValue(ClaimTypes.Uri))
                : (principal.FindFirstValue("urn:github:avatar_url") ?? principal.FindFirstValue(ClaimTypes.Uri));

            if (isGoogle)
                await _usersService.CreateGoogleUserAsync(email, displayName, avatarUrl);
            else
                await _usersService.CreateGitHubUserAsync(email, displayName, avatarUrl);

            var userEntity = await _usersDataController.GetUserByEmail(email);
            if (userEntity == null)
                return null;

            var expectedProvider = isGoogle ? OAuthProvider.Google : OAuthProvider.GitHub;

            if (userEntity.OAuthProvider == expectedProvider)
            {
                var token = await GenerateTokenForUser(userEntity.Id, userEntity.UserRole!.Value, keepSignedIn: true);
                return token == null ? null : new OAuthExchangeResult { Token = token };
            }

            // Email already registered with a different provider → verify identity via 2FA
            var pending = await _twoFactorService.SendCode(userEntity);
            return new OAuthExchangeResult { PendingTwoFactor = pending };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Provider} login failed", providerName);
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
