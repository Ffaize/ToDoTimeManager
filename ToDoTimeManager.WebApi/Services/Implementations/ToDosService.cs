using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations
{
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

        public async Task<ToDo?> GetToDoById(Guid toDoId)
        {
            try
            {
                var res = await _toDosDataController.GetToDoById(toDoId);
                return res?.ToToDo();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<List<ToDo>> GetToDosByUserId(Guid userId)
        {
            try
            {
                var res = await _toDosDataController.GetToDosByUserId(userId);
                return res.Select(tde => tde.ToToDo()).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return [];
            }
        }

        public async Task<bool> CreateToDo(ToDo newToDo)
        {
            try
            {
                return await _toDosDataController.CreateToDo(new ToDoEntity(newToDo));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> UpdateToDo(ToDo updatedToDo)
        {
            try
            {
                return await _toDosDataController.UpdateToDo(new ToDoEntity(updatedToDo));
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
                return await _toDosDataController.DeleteToDo(toDoId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }
    }
}
