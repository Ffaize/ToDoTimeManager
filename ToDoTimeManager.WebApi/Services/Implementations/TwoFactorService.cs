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
    private readonly ITwoFactorCodeGeneratorService _codeGeneratorService;
    private readonly IEmailService _emailService;
    private readonly IJwtGeneratorService _jwtGeneratorService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TwoFactorService> _logger;

    public TwoFactorService(
        ITwoFactorCodesDataController twoFactorCodesDataController,
        IUsersDataController usersDataController,
        ITwoFactorCodeGeneratorService codeGeneratorService,
        IEmailService emailService,
        IJwtGeneratorService jwtGeneratorService,
        IConfiguration configuration,
        ILogger<TwoFactorService> logger)
    {
        _twoFactorCodesDataController = twoFactorCodesDataController;
        _usersDataController = usersDataController;
        _codeGeneratorService = codeGeneratorService;
        _emailService = emailService;
        _jwtGeneratorService = jwtGeneratorService;
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
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["TwoFactorSettings:CodeLifetimeMinutes"] ?? "5"))
        };

        var upsertSucceeded = await _twoFactorCodesDataController.UpsertCode(entity);
        if (!upsertSucceeded)
            throw new ConflictException("Failed to persist verification code.");
        await _emailService.SendTwoFactorCodeAsync(userEntity.Email, code);

        return new TwoFactorPendingModel
        {
            UserId = userId,
            MaskedEmail = userEntity.Email.MaskAsEmail()
        };
    }

    public async Task<TokenModel> VerifyCode(Guid userId, string code)
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

        if (!string.Equals(record.Code, code, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Invalid verification code.");

        var deleted = await _twoFactorCodesDataController.DeleteByUserId(userId);
        if (!deleted)
            throw new ConflictException("Failed to invalidate verification code. Please try again.");

        var userEntity = await _usersDataController.GetUserById(userId);
        if (userEntity == null)
            throw new NotFoundException("User not found");

        var user = userEntity.ToUser();
        return new TokenModel
        {
            AccessToken = _jwtGeneratorService.GenerateAccessToken(user.Id.ToString(), user.UserRole),
            RefreshToken = _jwtGeneratorService.GenerateRefreshToken(),
            RefreshTokenExpiresAt =
                DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenLifetime"] ?? "7"))
        };
    }

}
