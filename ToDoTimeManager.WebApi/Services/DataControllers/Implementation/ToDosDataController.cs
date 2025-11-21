using Dapper;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.DbAccessServices;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation;

public class ToDosDataController : IToDosDataController
{

    private readonly IDbAccessService _dbAccessService;
    private readonly ILogger<ToDosDataController> _logger;

    public ToDosDataController(IDbAccessService dbAccessService, ILogger<ToDosDataController> logger)
    {
        _dbAccessService = dbAccessService;
        _logger = logger;
    }
    public async Task<List<ToDoEntity>> GetAllToDos()
    {
        try
        {
            return await _dbAccessService.GetAllRecords<ToDoEntity>("sp_ToDos_GetAll");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<ToDoEntity?> GetToDoById(Guid toDoId)
    {
        try
        {
            return await _dbAccessService.GetRecordById<ToDoEntity>("sp_ToDos_GetById", toDoId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<ToDoEntity>> GetToDosByUserId(Guid userId)
    {
        try
        {
            return await _dbAccessService.GetAllByParameter<ToDoEntity>("sp_ToDos_GetByUserId", "AssignedTo", userId);
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
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("ToDoStatus", status);

            var result = await _dbAccessService.GetRecordsByParameters<ToDoEntity>("sp_ToDos_GetCountByUserIdAndStatus", parameters);
            return result.Count;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return 0;
        }
    }

    public async Task<bool> CreateToDo(ToDoEntity newToDo)
    {
        try
        {
            return await _dbAccessService.AddRecord("sp_ToDos_Create", newToDo) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateToDo(ToDoEntity updatedToDo)
    {
        try
        {
            return await _dbAccessService.UpdateRecord("sp_ToDos_Update", updatedToDo) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteToDo(Guid toDoId)
    {
        try
        {
            return await _dbAccessService.DeleteRecordById("sp_ToDos_Delete", toDoId) >= 1;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}