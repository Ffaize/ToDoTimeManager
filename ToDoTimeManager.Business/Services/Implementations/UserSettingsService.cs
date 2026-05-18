using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Entities.Exceptions;
using ToDoTimeManager.Shared.DTOs.UserSettings;

namespace ToDoTimeManager.Business.Services.Implementations;

public class UserSettingsService : IUserSettingsService
{
    private readonly IUserSettingsDataController _userSettingsDataController;
    private readonly ILogger<UserSettingsService> _logger;

    public UserSettingsService(
        IUserSettingsDataController userSettingsDataController,
        ILogger<UserSettingsService> logger)
    {
        _userSettingsDataController = userSettingsDataController;
        _logger                     = logger;
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
                IsTwoFactorEnabled = entity.IsTwoFactorEnabled
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
}
