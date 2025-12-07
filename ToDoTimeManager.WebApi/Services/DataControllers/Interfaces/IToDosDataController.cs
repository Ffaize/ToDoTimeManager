using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;

namespace ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;

public interface IToDosDataController
{
    Task<List<ToDoEntity>> GetAllToDos();
    Task<ToDoEntity?> GetToDoById(Guid toDoId);
    Task<List<ToDoEntity>> GetToDosByUserId(Guid userId);
    Task<int> GetToDosCountByUserIdAndStatus(Guid userId, ToDoStatus status);
    Task<bool> CreateToDo(ToDoEntity newToDo);
    Task<bool> UpdateToDo(ToDoEntity updatedToDo);
    Task<bool> DeleteToDo(Guid toDoId);
    Task<List<ToDo>> GetToDosByNearestDueDateByUserId(Guid filterUserId);
}