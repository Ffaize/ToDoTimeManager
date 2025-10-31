using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Implementation
{
    public class ToDosDataController : IToDosDataController
    {
        public async Task<List<ToDoEntity>> GetAllToDos()
        {
            throw new NotImplementedException();
        }

        public async Task<ToDoEntity?> GetToDoById(Guid toDoId)
        {
            throw new NotImplementedException();
        }

        public async Task<ToDoEntity> CreateToDo(ToDoEntity newToDo)
        {
            throw new NotImplementedException();
        }

        public async Task<ToDoEntity?> UpdateToDo(ToDoEntity updatedToDo)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteToDo(Guid toDoId)
        {
            throw new NotImplementedException();
        }
    }
}
