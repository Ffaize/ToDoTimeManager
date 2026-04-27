using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;

namespace ToDoTimeManager.WebApi.Services.Interfaces;

public interface IToDosService
{
    Task<List<ToDo>> GetAllToDos();
    Task<ToDo?> GetToDoById(Guid toDoId, Guid currentUserId, bool isAdmin);
    Task<List<ToDo>> GetToDosByUserId(Guid userId, Guid currentUserId, bool isAdmin);
    Task<List<ToDo>> GetToDosByTeamId(Guid teamId);
    Task<List<ToDo>> GetToDosByProjectId(Guid projectId);
    Task<List<ToDo>> GetToDosByNearestDueDateByUserId(Guid userId);
    Task<int> GetToDosCountByUserIdAndStatus(Guid userId, ToDoStatus status);
    Task<bool> CreateToDo(ToDo newToDo);
    Task<bool> UpdateToDo(ToDo updatedToDo, Guid currentUserId, bool isAdmin);
    Task<bool> DeleteToDo(Guid toDoId, Guid currentUserId, bool isAdmin);
}