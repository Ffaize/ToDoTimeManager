using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class ToDosService : IToDosService
{
    private readonly IToDosDataController _toDosDataController;
    private readonly ILogger<ToDosService> _logger;

    public ToDosService(IToDosDataController toDosDataController, ILogger<ToDosService> logger)
    {
        _toDosDataController = toDosDataController;
        _logger = logger;
    }

    public async Task<List<ToDo>> GetAllToDos()
    {
        try
        {
            var res = await _toDosDataController.GetAllToDos();
            return res.Select(tde => tde.ToToDo()).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<ToDo?> GetToDoById(Guid toDoId, Guid currentUserId, bool isAdmin)
    {
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
            var res = await _toDosDataController.GetToDoById(toDoId);
            if (res == null)
                throw new NotFoundException("To-do was not found");

            if (res.AssignedTo != currentUserId && !isAdmin)
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

    public async Task<List<ToDo>> GetToDosByUserId(Guid userId, Guid currentUserId, bool isAdmin)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (userId != currentUserId && !isAdmin)
            throw new ForbiddenException();

        try
        {
            var res = await _toDosDataController.GetToDosByUserId(userId);
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

    public async Task<bool> CreateToDo(ToDo newToDo)
    {
        if (string.IsNullOrWhiteSpace(newToDo.Title))
            throw new ValidationException("To-do title is required");

        try
        {
            return await _toDosDataController.CreateToDo(new ToDoEntity(newToDo));
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

    public async Task<bool> UpdateToDo(ToDo updatedToDo, Guid currentUserId, bool isAdmin)
    {
        try
        {
            var existing = await _toDosDataController.GetToDoById(updatedToDo.Id);
            if (existing == null)
                throw new NotFoundException("To-do was not found");

            if (existing.AssignedTo != currentUserId && !isAdmin)
                throw new ForbiddenException();

            return await _toDosDataController.UpdateToDo(new ToDoEntity(updatedToDo));
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

    public async Task<bool> DeleteToDo(Guid toDoId, Guid currentUserId, bool isAdmin)
    {
        if (toDoId == Guid.Empty)
            throw new ValidationException("Invalid to-do ID");

        try
        {
            var existing = await _toDosDataController.GetToDoById(toDoId);
            if (existing == null)
                throw new NotFoundException("To-do was not found");

            if (existing.AssignedTo != currentUserId && !isAdmin)
                throw new ForbiddenException();

            return await _toDosDataController.DeleteToDo(toDoId);
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
