using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.DbAccessServices;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation
{
    public class ToDosDataController : IToDosDataController
    {

        private readonly IDbAccessService _dbAccessService;
        private readonly ILogger<UsersDataController> _logger;

        public ToDosDataController(IDbAccessService dbAccessService, ILogger<UsersDataController> logger)
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
}
