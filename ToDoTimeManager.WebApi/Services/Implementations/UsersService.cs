using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class UsersService : IUsersService
{

    private readonly IUsersDataController _usersDataController;
    private readonly ILogger<UsersService> _logger;
    private readonly IPasswordHelperService _passwordHelperService;

    public UsersService(IUsersDataController usersDataController, ILogger<UsersService> logger, IPasswordHelperService passwordHelperService)
    {
        _usersDataController = usersDataController;
        _logger = logger;
        _passwordHelperService = passwordHelperService;
    }

    public async Task<List<User>> GetAllUsers()
    {
        try
        {
            var res = await _usersDataController.GetAllUsers();
            return res.Select(ue => ue.ToUser()).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        try
        {
            var res = await _usersDataController.GetUserById(userId);
            return res?.ToUser();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        try
        {
            var res = await _usersDataController.GetUserByUsername(username);
            return res?.ToUser();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        try
        {
            var res = await _usersDataController.GetUserByEmail(email);
            return res?.ToUser();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByLoginParameter(string loginParameter)
    {
        try
        {
            var res = await _usersDataController.GetUserByLoginParameter(loginParameter);
            return res?.ToUser();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> CreateUser(User newUser)
    {
        try
        {
            newUser.Password = _passwordHelperService.HashPassword(newUser.Id.ToString(), newUser.Password!);
            return await _usersDataController.CreateUser(new UserEntity(newUser));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateUser(User updatedUser)
    {
        try
        {
            return await _usersDataController.UpdateUser(new UserEntity(updatedUser));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        try
        {
            return await _usersDataController.DeleteUser(userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}