using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Exceptions;
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
            throw new ServiceException("Failed to persist verification code.");
        await _emailService.SendTwoFactorCodeAsync(userEntity.Email!, code);

        return new TwoFactorPendingModel
        {
            UserId = userId,
            MaskedEmail = MaskEmail(userEntity.Email!)
        };
    }

    public async Task<TokenModel> VerifyCode(Guid userId, string code)
    {
        var record = await _twoFactorCodesDataController.GetByUserId(userId);
        if (record == null)
            throw new ValidationException("No verification code found. Please request a new one.");

        if (record.ExpiresAt < DateTime.UtcNow)
        {
            await _twoFactorCodesDataController.DeleteByUserId(userId);
            throw new ValidationException("Verification code has expired. Please request a new one.");
        }

        if (!string.Equals(record.Code, code, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Invalid verification code.");

        await _twoFactorCodesDataController.DeleteByUserId(userId);

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

    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 1) return email;
        return $"{email[0]}***{email[atIndex..]}";
    }
}
