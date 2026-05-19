using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Entities.Exceptions;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Implementations;

public class PasswordResetService : IPasswordResetService
{
    private readonly IPasswordResetsDataController _passwordResetsDataController;
    private readonly IUsersDataController _usersDataController;
    private readonly IUserSecretsDataController _userSecretsDataController;
    private readonly ITwoFactorCodesHelper _codeHasher;
    private readonly IPasswordHelperService _passwordHelperService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PasswordResetService> _logger;

    public PasswordResetService(
        IPasswordResetsDataController passwordResetsDataController,
        IUsersDataController usersDataController,
        IUserSecretsDataController userSecretsDataController,
        ITwoFactorCodesHelper codeHasher,
        IPasswordHelperService passwordHelperService,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<PasswordResetService> logger)
    {
        _passwordResetsDataController = passwordResetsDataController;
        _usersDataController = usersDataController;
        _userSecretsDataController = userSecretsDataController;
        _codeHasher = codeHasher;
        _passwordHelperService = passwordHelperService;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<PasswordResetPendingModel> SendResetCode(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email is required.");

        var lifetimeMinutes = int.TryParse(_configuration["TwoFactorSettings:CodeLifetimeMinutes"], out var m) ? m : 5;

        var userEntity = await _usersDataController.GetUserByEmail(email);

        // Always return a successful response to prevent user enumeration
        if (userEntity == null)
        {
            return new PasswordResetPendingModel
            {
                UserId = Guid.Empty,
                MaskedEmail = MaskEmail(email),
                CodeLifetimeSeconds = lifetimeMinutes * 60
            };
        }

        var code = _codeHasher.GenerateCode();
        var entity = new PasswordResetEntity
        {
            Id = Guid.NewGuid(),
            UserId = userEntity.Id,
            Code = _codeHasher.HashCode(code),
            ExpiresAt = DateTime.UtcNow.AddMinutes(lifetimeMinutes)
        };

        var upsertSucceeded = await _passwordResetsDataController.Upsert(entity);
        if (!upsertSucceeded)
            throw new ConflictException("Failed to persist reset code.");

        await _emailService.SendPasswordResetCodeAsync(userEntity.Email!, code);

        return new PasswordResetPendingModel
        {
            UserId = userEntity.Id,
            MaskedEmail = MaskEmail(userEntity.Email!),
            CodeLifetimeSeconds = lifetimeMinutes * 60
        };
    }

    public async Task ResetPassword(Guid userId, string code, string newPassword)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid reset session.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ValidationException("Code is required.");

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            throw new ValidationException("Password must be at least 8 characters.");

        var record = await _passwordResetsDataController.GetByUserId(userId);
        if (record == null)
            throw new ValidationException("Reset code not found or already used.");

        if (record.ExpiresAt < DateTime.UtcNow)
        {
            await _passwordResetsDataController.DeleteByUserId(userId);
            throw new ValidationException("Reset code has expired. Please request a new one.");
        }

        if (!_codeHasher.VerifyCode(code, record.Code))
            throw new ValidationException("Invalid reset code.");

        var newSalt = _passwordHelperService.GenerateSalt();
        var newHash = _passwordHelperService.HashPassword(newSalt, newPassword);

        var updated = await _userSecretsDataController.UpdatePassword(userId, newHash, newSalt);
        if (!updated)
            throw new ConflictException("Failed to update password. Please try again.");

        await _passwordResetsDataController.DeleteByUserId(userId);
    }

    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 1)
            return email;

        var local = email[..atIndex];
        var domain = email[atIndex..];
        var visible = local.Length > 2 ? 2 : 1;
        return local[..visible] + new string('*', local.Length - visible) + domain;
    }
}
