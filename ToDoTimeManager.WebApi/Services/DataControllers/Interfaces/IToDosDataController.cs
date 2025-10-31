using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface IToDosDataController
{
    Task<List<ToDoEntity>> GetAllToDos();
    Task<ToDoEntity?> GetToDoById(Guid toDoId);
    Task<bool> CreateToDo(ToDoEntity newToDo);
    Task<bool> UpdateToDo(ToDoEntity updatedToDo);
    Task<bool> DeleteToDo(Guid toDoId);
}