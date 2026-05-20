using ToDoTimeManager.Business.Utils.Implementations;
using ToDoTimeManager.Business.Utils.Interfaces;
using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Entities.Exceptions;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Implementations;

public class TwoFactorService : ITwoFactorService
{
    private readonly ITwoFactorCodesDataController  _twoFactorCodesDataController;
    private readonly IUsersDataController           _usersDataController;
    private readonly IUserSecretsDataController     _userSecretsDataController;
    private readonly IUserSettingsDataController    _userSettingsDataController;
    private readonly IUserTotpSecretsDataController _userTotpSecretsDataController;
    private readonly ITwoFactorCodesHelper          _twoFactorCodesHelper;
    private readonly IEmailService                  _emailService;
    private readonly IJwtGeneratorService           _jwtGeneratorService;
    private readonly ITotpService                   _totpService;
    private readonly IConfiguration                 _configuration;
    private readonly ILogger<TwoFactorService>      _logger;

    public TwoFactorService(
        ITwoFactorCodesDataController twoFactorCodesDataController,
        IUsersDataController usersDataController,
        IUserSecretsDataController userSecretsDataController,
        IUserSettingsDataController userSettingsDataController,
        IUserTotpSecretsDataController userTotpSecretsDataController,
        ITwoFactorCodesHelper twoFactorCodesHelper,
        IEmailService emailService,
        IJwtGeneratorService jwtGeneratorService,
        ITotpService totpService,
        IConfiguration configuration,
        ILogger<TwoFactorService> logger)
    {
        _twoFactorCodesDataController  = twoFactorCodesDataController;
        _usersDataController           = usersDataController;
        _userSecretsDataController     = userSecretsDataController;
        _userSettingsDataController    = userSettingsDataController;
        _userTotpSecretsDataController = userTotpSecretsDataController;
        _twoFactorCodesHelper          = twoFactorCodesHelper;
        _emailService                  = emailService;
        _jwtGeneratorService           = jwtGeneratorService;
        _totpService                   = totpService;
        _configuration                 = configuration;
        _logger                        = logger;
    }

    public async Task<TwoFactorPendingModel> SendCode(UserEntity user)
    {
        if (user == null)
            throw new NotFoundException("User not found");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ValidationException("User has no email address configured.");

        var method = await _userSettingsDataController.GetTwoFactorMethod(user.Id) ?? 0;

        if (method == (int)TwoFactorMethod.AuthApp)
        {
            return new TwoFactorPendingModel
            {
                UserId            = user.Id,
                Email             = user.Email,
                CodeLifetimeSeconds = 30,
                SenderEmail       = null,
                TwoFactorMethod   = TwoFactorMethod.AuthApp
            };
        }

        var code = _twoFactorCodesHelper.GenerateCode();
        var entity = new TwoFactorCodeEntity
        {
            Id        = Guid.NewGuid(),
            UserId    = user.Id,
            Code      = _twoFactorCodesHelper.HashCode(code),
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["TwoFactorSettings:CodeLifetimeMinutes"] ?? "5"))
        };

        var upsertSucceeded = await _twoFactorCodesDataController.UpsertCode(entity);
        if (!upsertSucceeded)
            throw new ConflictException("Failed to persist verification code.");

        await _emailService.SendTwoFactorCodeAsync(user.Email, code);

        var lifetimeMinutes = int.Parse(_configuration["TwoFactorSettings:CodeLifetimeMinutes"] ?? "5");
        return new TwoFactorPendingModel
        {
            UserId              = user.Id,
            Email               = user.Email,
            CodeLifetimeSeconds = lifetimeMinutes * 60,
            SenderEmail         = _configuration["EmailSettings:SenderEmail"],
            TwoFactorMethod     = TwoFactorMethod.Email
        };
    }

    public async Task<TokenModel> VerifyCode(Guid userId, string code, bool keepSignedIn)
    {
        var method = await _userSettingsDataController.GetTwoFactorMethod(userId) ?? 0;

        if (method == (int)TwoFactorMethod.AuthApp)
        {
            var secretEntity = await _userTotpSecretsDataController.GetByUserId(userId);
            if (secretEntity == null)
                throw new NotFoundException("TOTP secret not configured for this user.");

            if (!_totpService.VerifyCode(secretEntity.Secret, code))
                throw new ValidationException("Invalid authenticator code.");

            return await IssueTokenAsync(userId, keepSignedIn);
        }

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

        if (!_twoFactorCodesHelper.VerifyCode(code, record.Code))
            throw new ValidationException("Invalid verification code.");

        var deleted = await _twoFactorCodesDataController.DeleteByUserId(userId);
        if (!deleted)
            throw new ConflictException("Failed to invalidate verification code. Please try again.");

        return await IssueTokenAsync(userId, keepSignedIn);
    }

    private async Task<TokenModel> IssueTokenAsync(Guid userId, bool keepSignedIn)
    {
        var userRole = await _usersDataController.GetUserRoleByUserId(userId);
        if (userRole is null)
            throw new NotFoundException("User not found");

        string? plainRefreshToken = null;
        DateTime? refreshExpiresAt = null;

        if (keepSignedIn)
        {
            var rtDays = int.TryParse(_configuration["JwtSettings:RefreshTokenLifetime"], out var days) ? days : 14;
            plainRefreshToken = _jwtGeneratorService.GenerateRefreshToken();
            refreshExpiresAt  = DateTime.UtcNow.AddDays(rtDays);
            await _userSecretsDataController.UpdateRefreshToken(
                userId, HashHelper.HashRefreshToken(plainRefreshToken), refreshExpiresAt);
        }
        else
        {
            await _userSecretsDataController.ClearRefreshToken(userId);
        }

        return new TokenModel
        {
            AccessToken          = _jwtGeneratorService.GenerateAccessToken(userId.ToString(), userRole),
            RefreshToken         = plainRefreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }
}
