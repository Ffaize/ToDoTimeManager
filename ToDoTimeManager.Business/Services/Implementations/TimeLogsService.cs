using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Entities.Exceptions;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Implementations;

public class TimeLogsService : ITimeLogsService
{
    private readonly ITimeLogsDataController _timeLogsDataController;
    private readonly IAccessControlDataController _accessControlDataController;
    private readonly IActivityLogsService _activityLogsService;
    private readonly ILogger<TimeLogsService> _logger;

    public TimeLogsService(
        ITimeLogsDataController timeLogsDataController,
        IAccessControlDataController accessControlDataController,
        IActivityLogsService activityLogsService,
        ILogger<TimeLogsService> logger)
    {
        _timeLogsDataController      = timeLogsDataController;
        _accessControlDataController = accessControlDataController;
        _activityLogsService         = activityLogsService;
        _logger                      = logger;
    }

    public async Task<List<TimeLog>> GetAllTimeLogs()
    {
        try
        {
            List<TimeLogEntity> res = await _timeLogsDataController.GetAllTimeLogs();
            return res.Select(tle => tle.ToTimeLog()).ToList()!;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<TimeLog?> GetTimeLogById(Guid timeLogId, Guid currentUserId, UserRole currentUserRole)
    {
        if (timeLogId == Guid.Empty)
            throw new ValidationException("Invalid time log ID");

        try
        {
            var res = await _timeLogsDataController.GetTimeLogById(timeLogId);
            if (res == null)
                throw new NotFoundException("Time log was not found");

            if (currentUserRole < UserRole.Manager && !await _accessControlDataController.CanAccessTimeLog(currentUserId, timeLogId))
                throw new ForbiddenException();

            return res.ToTimeLog();
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

    public async Task<List<TimeLog>> GetTimeLogsByToDoId(Guid toDoId, Guid currentUserId, UserRole currentUserRole)
    {
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
            if (currentUserRole < UserRole.Manager && !await _accessControlDataController.CanAccessToDo(currentUserId, toDoId))
                throw new ForbiddenException();

            List<TimeLogEntity> res = await _timeLogsDataController.GetTimeLogsByToDoId(toDoId);
            return res.Select(tle => tle.ToTimeLog()).ToList()!;
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

    public async Task<List<TimeLog>> GetTimeLogsByUserId(Guid userId, Guid currentUserId, UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            if (currentUserRole < UserRole.Manager && userId != currentUserId)
                throw new ForbiddenException();

            List<TimeLogEntity> res = await _timeLogsDataController.GetTimeLogsByUserId(userId);
            return res.Select(tle => tle.ToTimeLog()).ToList()!;
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

    public async Task<List<TimeLog>> GetTimeLogsByUserIdAndToDoId(Guid toDoId, Guid userId, Guid currentUserId,
        UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
            if (currentUserRole < UserRole.Manager && userId != currentUserId)
                throw new ForbiddenException();

            if (currentUserRole < UserRole.Manager && !await _accessControlDataController.CanAccessToDo(currentUserId, toDoId))
                throw new ForbiddenException();

            List<TimeLogEntity> res = await _timeLogsDataController.GetTimeLogsByUserIdAndToDoId(toDoId, userId);
            return res.Select(tle => tle.ToTimeLog()).ToList()!;
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

    public async Task<List<TimeLog>> GetTimeLogsByUserIdAndTime(Guid userId, int daysAgo)
    {
        try
        {
            List<TimeLogEntity> res = await _timeLogsDataController.GetTimeLogsByUserIdAndTime(userId, daysAgo);
            return res.Select(tle => tle.ToTimeLog()).ToList()!;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<bool> CreateTimeLog(TimeLog newTimeLog, Guid currentUserId, UserRole currentUserRole)
    {
        try
        {
            if (currentUserRole < UserRole.Manager && !await _accessControlDataController.CanAccessToDo(currentUserId, newTimeLog.ToDoId))
                throw new ForbiddenException();

            var result = await _timeLogsDataController.CreateTimeLog(new TimeLogEntity(newTimeLog));
            if (result)
            {
                var hours = newTimeLog.HoursSpent.ToString("F1");
                var desc = string.IsNullOrWhiteSpace(newTimeLog.LogDescription)
                    ? $"logged {hours}h"
                    : $"logged {hours}h: {newTimeLog.LogDescription}";
                _ = _activityLogsService.LogActivity(newTimeLog.ToDoId, newTimeLog.UserId, ActivityType.TimeLogged, desc);
            }
            return result;
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

    public async Task<bool> UpdateTimeLog(TimeLog updatedTimeLog, Guid currentUserId, UserRole currentUserRole)
    {
        try
        {
            var existing = await _timeLogsDataController.GetTimeLogById(updatedTimeLog.Id);
            if (existing == null)
                throw new NotFoundException("Time log was not found");

            if (currentUserRole < UserRole.Manager && !await _accessControlDataController.CanAccessTimeLog(currentUserId, updatedTimeLog.Id))
                throw new ForbiddenException();

            var result = await _timeLogsDataController.UpdateTimeLog(new TimeLogEntity(updatedTimeLog));
            if (result)
            {
                var hours = updatedTimeLog.HoursSpent.ToString("F1");
                _ = _activityLogsService.LogActivity(existing.ToDoId, currentUserId, ActivityType.TimeLogUpdated, $"updated time log ({hours}h)");
            }
            return result;
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

    public async Task<bool> DeleteTimeLog(Guid timeLogId, Guid currentUserId, UserRole currentUserRole)
    {
        if (timeLogId == Guid.Empty)
            throw new ValidationException("Invalid time log ID");

        try
        {
            var existing = await _timeLogsDataController.GetTimeLogById(timeLogId);
            if (existing == null)
                throw new NotFoundException("Time log was not found");

            if (currentUserRole < UserRole.Manager && !await _accessControlDataController.CanAccessTimeLog(currentUserId, timeLogId))
                throw new ForbiddenException();

            var result = await _timeLogsDataController.DeleteTimeLog(timeLogId);
            if (result)
                _ = _activityLogsService.LogActivity(existing.ToDoId, currentUserId, ActivityType.TimeLogDeleted, "deleted time log");
            return result;
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
