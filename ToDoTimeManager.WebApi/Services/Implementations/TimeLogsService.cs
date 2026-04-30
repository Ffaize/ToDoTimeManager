using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class TimeLogsService : ITimeLogsService
{
    private readonly ITimeLogsDataController _timeLogsDataController;
    private readonly ILogger<TimeLogsService> _logger;

    public TimeLogsService(ITimeLogsDataController timeLogsDataController, ILogger<TimeLogsService> logger)
    {
        _timeLogsDataController = timeLogsDataController;
        _logger = logger;
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

            if (res.UserId != currentUserId && currentUserRole < UserRole.Admin)
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

    public async Task<List<TimeLog>> GetTimeLogsByToDoId(Guid toDoId)
    {
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
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
        if (userId != currentUserId && currentUserRole < UserRole.Admin)
            throw new ForbiddenException();

        try
        {
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
        if (userId != currentUserId && currentUserRole < UserRole.Admin)
            throw new ForbiddenException();

        try
        {
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

    public async Task<bool> CreateTimeLog(TimeLog newTimeLog)
    {
        try
        {
            return await _timeLogsDataController.CreateTimeLog(new TimeLogEntity(newTimeLog));
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

            if (existing.UserId != currentUserId && currentUserRole < UserRole.Admin)
                throw new ForbiddenException();

            return await _timeLogsDataController.UpdateTimeLog(new TimeLogEntity(updatedTimeLog));
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

            if (existing.UserId != currentUserId && currentUserRole < UserRole.Admin)
                throw new ForbiddenException();

            return await _timeLogsDataController.DeleteTimeLog(timeLogId);
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
