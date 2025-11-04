using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IToDosService
{
    Task<List<ToDo>> GetAllToDos();
    Task<ToDo?> GetToDoById(Guid toDoId);
    Task<List<ToDo>> GetToDosByUserId(Guid userId);
    Task<bool> CreateToDo(ToDo newToDo);
    Task<bool> UpdateToDo(ToDo updatedToDo);
    Task<bool> DeleteToDo(Guid toDoId);
}