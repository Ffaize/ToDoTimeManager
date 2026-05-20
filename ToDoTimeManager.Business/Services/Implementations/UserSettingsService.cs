using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Business.Utils.Interfaces;
using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Entities.Exceptions;
using ToDoTimeManager.Shared.DTOs.UserSettings;
using ToDoTimeManager.Shared.Enums;

namespace ToDoTimeManager.Business.Services.Implementations;

public class UserSettingsService : IUserSettingsService
{
    private readonly IUserSettingsDataController    _userSettingsDataController;
    private readonly IUserTotpSecretsDataController _userTotpSecretsDataController;
    private readonly IUsersDataController           _usersDataController;
    private readonly ITotpService                   _totpService;
    private readonly ILogger<UserSettingsService>   _logger;

    public UserSettingsService(
        IUserSettingsDataController userSettingsDataController,
        IUserTotpSecretsDataController userTotpSecretsDataController,
        IUsersDataController usersDataController,
        ITotpService totpService,
        ILogger<UserSettingsService> logger)
    {
        _userSettingsDataController    = userSettingsDataController;
        _userTotpSecretsDataController = userTotpSecretsDataController;
        _usersDataController           = usersDataController;
        _totpService                   = totpService;
        _logger                        = logger;
    }

    public async Task<UserSettings?> GetUserSettings(Guid userId, Guid currentUserId, UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        if (userId != currentUserId && currentUserRole < UserRole.Admin)
            throw new ForbiddenException();

        try
        {
            var entity = await _userSettingsDataController.GetByUserId(userId);
            if (entity is null)
                throw new NotFoundException("User settings not found");

            return new UserSettings
            {
                UserId             = entity.UserId,
                IsTwoFactorEnabled = entity.IsTwoFactorEnabled,
                TwoFactorMethod    = (TwoFactorMethod)entity.TwoFactorMethod
            };
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> UpdateUserSettings(Guid userId, UpdateUserSettingsRequestDto request)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            return await _userSettingsDataController.Update(new UserSettingsEntity
            {
                UserId             = userId,
                IsTwoFactorEnabled = request.IsTwoFactorEnabled
            });
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool?> GetTwoFactorEnabled(Guid userId, Guid currentUserId, UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        if (userId != currentUserId && currentUserRole < UserRole.Admin)
            throw new ForbiddenException();

        try
        {
            return await _userSettingsDataController.GetTwoFactorEnabled(userId);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> SetTwoFactorEnabled(Guid userId, bool isEnabled)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            return await _userSettingsDataController.SetTwoFactorEnabled(userId, isEnabled);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<TotpSetupResponseDto> GenerateTotpSetup(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        var user = await _usersDataController.GetUserById(userId);
        if (user is null)
            throw new NotFoundException("User not found");

        var secret      = _totpService.GenerateSecret();
        var accountName = user.Email ?? user.UserName ?? userId.ToString();
        var uri         = _totpService.BuildQrCodeUri(accountName, secret);
        var qrBase64    = _totpService.GenerateQrCodeBase64(uri);

        return new TotpSetupResponseDto
        {
            QrCodeBase64 = qrBase64,
            Secret       = secret,
            AccountName  = accountName
        };
    }

    public async Task ConfirmTotpSetup(Guid userId, string secret, string code)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        if (!_totpService.VerifyCode(secret, code))
            throw new ValidationException("Invalid TOTP code. Please try again.");

        var upserted = await _userTotpSecretsDataController.Upsert(userId, secret);
        if (!upserted)
            throw new ConflictException("Failed to store TOTP secret.");

        await _userSettingsDataController.SetTwoFactorMethod(userId, (int)TwoFactorMethod.AuthApp);
        await _userSettingsDataController.SetTwoFactorEnabled(userId, true);
    }
}
