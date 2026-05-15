using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Implementations;

public class ActivityLogsService : IActivityLogsService
{
    private readonly IActivityLogsDataController _dataController;
    private readonly IAccessControlService _accessControlService;
    private readonly ILogger<ActivityLogsService> _logger;

    public ActivityLogsService(
        IActivityLogsDataController dataController,
        IAccessControlService accessControlService,
        ILogger<ActivityLogsService> logger)
    {
        _dataController       = dataController;
        _accessControlService = accessControlService;
        _logger               = logger;
    }

    public async Task<List<ActivityLog>> GetAllActivityLogs()
    {
        try
        {
            List<ActivityLogEntity> res = await _dataController.GetAllActivityLogs();
            return res.Select(e => e.ToActivityLog()).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<ActivityLog>> GetActivityLogsByToDoId(Guid toDoId, Guid currentUserId, UserRole currentUserRole)
    {
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, toDoId, nameof(GetActivityLogsByToDoId)))
                throw new ForbiddenException();

            List<ActivityLogEntity> res = await _dataController.GetActivityLogsByToDoId(toDoId);
            return res.Select(e => e.ToActivityLog()).ToList();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<ActivityLog>> GetActivityLogsByUserId(Guid userId, Guid currentUserId, UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, userId, nameof(GetActivityLogsByUserId)))
                throw new ForbiddenException();

            List<ActivityLogEntity> res = await _dataController.GetActivityLogsByUserId(userId);
            return res.Select(e => e.ToActivityLog()).ToList();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<ActivityLog>> GetActivityLogsByUserIdAndToDoId(Guid toDoId, Guid userId, Guid currentUserId, UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, toDoId, nameof(GetActivityLogsByUserIdAndToDoId)))
                throw new ForbiddenException();

            List<ActivityLogEntity> res = await _dataController.GetActivityLogsByUserIdAndToDoId(userId, toDoId);
            return res.Select(e => e.ToActivityLog()).ToList();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task LogActivity(Guid? toDoId, Guid userId, ActivityType type, string description)
    {
        try
        {
            var entry = new ActivityLogEntity
            {
                Id           = Guid.NewGuid(),
                ToDoId       = toDoId,
                UserId       = userId,
                Type         = type,
                Description  = description,
                ActivityTime = DateTime.UtcNow
            };
            await _dataController.CreateActivityLog(entry);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to log activity: {Message}", e.Message);
        }
    }
}
