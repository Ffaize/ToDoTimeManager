using System.Security.Cryptography;
using System.Text;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Extensions;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class TwoFactorService : ITwoFactorService
{
    private readonly ITwoFactorCodesDataController _twoFactorCodesDataController;
    private readonly IUsersDataController _usersDataController;
    private readonly IUserSecretsDataController _userSecretsDataController;
    private readonly ITwoFactorCodeGeneratorService _codeGeneratorService;
    private readonly IEmailService _emailService;
    private readonly IJwtGeneratorService _jwtGeneratorService;
    private readonly ITwoFactorCodeHasherService _codeHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TwoFactorService> _logger;

    public TwoFactorService(
        ITwoFactorCodesDataController twoFactorCodesDataController,
        IUsersDataController usersDataController,
        IUserSecretsDataController userSecretsDataController,
        ITwoFactorCodeGeneratorService codeGeneratorService,
        IEmailService emailService,
        IJwtGeneratorService jwtGeneratorService,
        ITwoFactorCodeHasherService codeHasher,
        IConfiguration configuration,
        ILogger<TwoFactorService> logger)
    {
        _twoFactorCodesDataController = twoFactorCodesDataController;
        _usersDataController = usersDataController;
        _userSecretsDataController = userSecretsDataController;
        _codeGeneratorService = codeGeneratorService;
        _emailService = emailService;
        _jwtGeneratorService = jwtGeneratorService;
        _codeHasher = codeHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TwoFactorPendingModel> SendCode(Guid userId)
    {
        var userEntity = await _usersDataController.GetUserById(userId);
        if (userEntity == null)
            throw new NotFoundException("User not found");

        if (string.IsNullOrWhiteSpace(userEntity.Email))
            throw new ValidationException("User has no email address configured.");

        var code = _codeGeneratorService.GenerateCode();
        var entity = new TwoFactorCodeEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Code = _codeHasher.HashCode(code),
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["TwoFactorSettings:CodeLifetimeMinutes"] ?? "5"))
        };

        var upsertSucceeded = await _twoFactorCodesDataController.UpsertCode(entity);
        if (!upsertSucceeded)
            throw new ConflictException("Failed to persist verification code.");
        await _emailService.SendTwoFactorCodeAsync(userEntity.Email, code);

        var lifetimeMinutes = int.Parse(_configuration["TwoFactorSettings:CodeLifetimeMinutes"] ?? "5");
        return new TwoFactorPendingModel
        {
            UserId = userId,
            MaskedEmail = userEntity.Email,
            CodeLifetimeSeconds = lifetimeMinutes * 60,
            SenderEmail = _configuration["EmailSettings:SenderEmail"]
        };
    }

    public async Task<TokenModel> VerifyCode(Guid userId, string code, bool keepSignedIn)
    {
        var record = await _twoFactorCodesDataController.GetByUserId(userId);
        if (record == null)
        {
            _logger.LogError("Unable to verify two-factor code for user {UserId}: code lookup returned null.", userId);
            throw new InvalidOperationException("Unable to verify the verification code at this time.");
        }

        if (record.ExpiresAt < DateTime.UtcNow)
        {
            await _twoFactorCodesDataController.DeleteByUserId(userId);
            throw new ValidationException("Verification code has expired. Please request a new one.");
        }

        if (!_codeHasher.VerifyCode(code, record.Code))
            throw new ValidationException("Invalid verification code.");

        var deleted = await _twoFactorCodesDataController.DeleteByUserId(userId);
        if (!deleted)
            throw new ConflictException("Failed to invalidate verification code. Please try again.");

        var userEntity = await _usersDataController.GetUserById(userId);
        if (userEntity == null)
            throw new NotFoundException("User not found");

        var user = userEntity.ToUser();

        string? plainRefreshToken = null;
        DateTime? refreshExpiresAt = null;

        if (keepSignedIn)
        {
            var rtDays = int.TryParse(_configuration["JwtSettings:RefreshTokenLifetime"], out var d) ? d : 14;
            plainRefreshToken = _jwtGeneratorService.GenerateRefreshToken();
            refreshExpiresAt = DateTime.UtcNow.AddDays(rtDays);
            await _userSecretsDataController.UpdateRefreshToken(
                userId, HashRefreshToken(plainRefreshToken), refreshExpiresAt);
        }
        else
        {
            await _userSecretsDataController.ClearRefreshToken(userId);
        }

        return new TokenModel
        {
            AccessToken = _jwtGeneratorService.GenerateAccessToken(user.Id.ToString(), user.UserRole),
            RefreshToken = plainRefreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }

    private static string HashRefreshToken(string plainToken) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(plainToken)));
}
