using ToDoTimeManager.DataAccess.DataControllers.Interfaces;
using ToDoTimeManager.Business.Services.Interfaces;
using ToDoTimeManager.Entities.Entities;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.Business.Services.Implementations;

public class ToDosService : IToDosService
{
    private readonly IToDosDataController _toDosDataController;
    private readonly IAccessControlService _accessControlService;
    private readonly IActivityLogsService _activityLogsService;
    private readonly ILogger<ToDosService> _logger;

    public ToDosService(
        IToDosDataController toDosDataController,
        IAccessControlService accessControlService,
        IActivityLogsService activityLogsService,
        ILogger<ToDosService> logger)
    {
        _toDosDataController  = toDosDataController;
        _accessControlService = accessControlService;
        _activityLogsService  = activityLogsService;
        _logger               = logger;
    }

    public async Task<List<ToDo>> GetAllToDos()
    {
        try
        {
            List<ToDoEntity> res = await _toDosDataController.GetAllToDos();
            return res.Select(tde => tde.ToToDo()).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<ToDo?> GetToDoById(Guid toDoId, Guid currentUserId, UserRole currentUserRole)
    {
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
            var res = await _toDosDataController.GetToDoById(toDoId);
            if (res == null)
                throw new NotFoundException("To-do was not found");

            if (!await _accessControlService.IsAccessibleToUser(currentUserId, toDoId, nameof(GetToDoById)))
                throw new ForbiddenException();

            return res.ToToDo();
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

    public async Task<List<ToDo>> GetToDosByUserId(Guid userId, Guid currentUserId, UserRole currentUserRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, userId, nameof(GetToDosByUserId)))
                throw new ForbiddenException();

            List<ToDoEntity> res = await _toDosDataController.GetToDosByUserId(userId);
            return res.Select(tde => tde.ToToDo()).ToList();
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

    public async Task<List<ToDo>> GetToDosByTeamId(Guid teamId)
    {
        try
        {
            List<ToDoEntity> res = await _toDosDataController.GetToDosByTeamId(teamId);
            return res.Select(e => e.ToToDo()).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<ToDo>> GetToDosByProjectId(Guid projectId)
    {
        try
        {
            List<ToDoEntity> res = await _toDosDataController.GetToDosByProjectId(projectId);
            return res.Select(e => e.ToToDo()).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<ToDo>> GetToDosByNearestDueDateByUserId(Guid userId)
    {
        try
        {
            return await _toDosDataController.GetToDosByNearestDueDateByUserId(userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<int> GetToDosCountByUserIdAndStatus(Guid userId, ToDoStatus status)
    {
        try
        {
            return await _toDosDataController.GetToDosCountByUserIdAndStatus(userId, status);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return 0;
        }
    }

    public async Task<bool> CreateToDo(ToDo newToDo, Guid currentUserId, UserRole currentUserRole)
    {
        if (string.IsNullOrWhiteSpace(newToDo.Title))
            throw new ValidationException("To-do title is required");

        try
        {
            var projectId = newToDo.ProjectId ?? Guid.Empty;
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, projectId, nameof(CreateToDo)))
                throw new ForbiddenException();

            var result = await _toDosDataController.CreateToDo(new ToDoEntity(newToDo));
            if (result)
                _ = _activityLogsService.LogActivity(newToDo.Id, currentUserId, ActivityType.ToDoCreated, "opened");
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

    public async Task<bool> UpdateToDo(ToDo updatedToDo, Guid currentUserId, UserRole currentUserRole)
    {
        try
        {
            var existing = await _toDosDataController.GetToDoById(updatedToDo.Id);
            if (existing == null)
                throw new NotFoundException("To-do was not found");

            var projectId = existing.ProjectId ?? Guid.Empty;
            if (!await _accessControlService.IsAccessibleToUser(currentUserId, projectId, nameof(UpdateToDo)))
                throw new ForbiddenException();

            bool statusChanged = existing.Status != updatedToDo.Status;
            var result = await _toDosDataController.UpdateToDo(new ToDoEntity(updatedToDo));
            if (result)
            {
                _ = _activityLogsService.LogActivity(updatedToDo.Id, currentUserId, ActivityType.ToDoUpdated, "updated");
                if (statusChanged)
                    _ = _activityLogsService.LogActivity(updatedToDo.Id, currentUserId, ActivityType.StatusChanged, $"moved to {updatedToDo.Status}");
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

    public async Task<bool> DeleteToDo(Guid toDoId, Guid currentUserId, UserRole currentUserRole)
    {
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
            var existing = await _toDosDataController.GetToDoById(toDoId);
            if (existing == null)
                throw new NotFoundException("To-do was not found");

            if (!await _accessControlService.IsAccessibleToUser(currentUserId, toDoId, nameof(DeleteToDo)))
                throw new ForbiddenException();

            var result = await _toDosDataController.DeleteToDo(toDoId);
            if (result)
                _ = _activityLogsService.LogActivity(toDoId, currentUserId, ActivityType.ToDoDeleted, $"deleted \"{existing.Title}\"");
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
