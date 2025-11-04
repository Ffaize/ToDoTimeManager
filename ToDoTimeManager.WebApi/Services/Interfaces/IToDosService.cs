using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IToDosService
{
    Task<List<ToDo>> GetAllToDos();
    Task<ToDo?> GetToDoById(Guid toDoId);
    Task<bool> CreateToDo(ToDo newToDo);
    Task<bool> UpdateToDo(ToDo updatedToDo);
    Task<bool> DeleteToDo(Guid toDoId);
}