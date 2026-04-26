using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IToDosService
{
    Task<List<ToDo>> GetAllToDos();
    Task<ToDo?> GetToDoById(Guid toDoId, Guid currentUserId, bool isAdmin);
    Task<List<ToDo>> GetToDosByUserId(Guid userId, Guid currentUserId, bool isAdmin);
    Task<bool> CreateToDo(ToDo newToDo);
    Task<bool> UpdateToDo(ToDo updatedToDo, Guid currentUserId, bool isAdmin);
    Task<bool> DeleteToDo(Guid toDoId, Guid currentUserId, bool isAdmin);
}
