using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Entities;
using ToDoTimeManager.WebApi.Exceptions;
using ToDoTimeManager.WebApi.Services.DataControllers.Interfaces;
using ToDoTimeManager.WebApi.Services.Interfaces;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IUsersDataController _usersDataController;
    private readonly ILogger<UsersService> _logger;
    private readonly IPasswordHelperService _passwordHelperService;

    public UsersService(IUsersDataController usersDataController, ILogger<UsersService> logger,
        IPasswordHelperService passwordHelperService)
    {
        _usersDataController = usersDataController;
        _logger = logger;
        _passwordHelperService = passwordHelperService;
    }

    public async Task<List<User>> GetAllUsers()
    {
        try
        {
            List<UserEntity> res = await _usersDataController.GetAllUsers();
            return res.Select(ue => ue.ToUser()).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<User?> GetUserById(Guid userId, Guid currentUserId, bool isAdmin)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            var res = await _usersDataController.GetUserById(userId);
            if (res == null)
                throw new NotFoundException("User was not found");

            if (userId != currentUserId && !isAdmin)
                throw new ForbiddenException();

            return res.ToUser();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ValidationException("Invalid username");

        try
        {
            var res = await _usersDataController.GetUserByUsername(username);
            if (res == null)
                throw new NotFoundException("User was not found");
            return res.ToUser();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Invalid email");

        try
        {
            var res = await _usersDataController.GetUserByEmail(email);
            if (res == null)
                throw new NotFoundException("User was not found");
            return res.ToUser();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<User?> GetUserByLoginParameter(string loginParameter)
    {
        if (string.IsNullOrWhiteSpace(loginParameter))
            throw new ValidationException("Invalid login parameter");

        try
        {
            var res = await _usersDataController.GetUserByLoginParameter(loginParameter);
            if (res == null)
                throw new NotFoundException("User was not found");
            return res.ToUser();
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> CreateUser(CreateUserRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new ValidationException("Username is required");
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required");

        try
        {
            var existingByUsername = await _usersDataController.GetUserByUsername(request.UserName);
            if (existingByUsername != null)
                throw new ConflictException("Username is already taken");

            var existingByEmail = await _usersDataController.GetUserByEmail(request.Email);
            if (existingByEmail != null)
                throw new ConflictException("Email is already registered");

            var user = new User
            {
                Id = request.Id,
                UserName = request.UserName,
                Email = request.Email,
                Password = _passwordHelperService.HashPassword(request.Id.ToString(), request.Password),
                UserRole = UserRole.User
            };

            return await _usersDataController.CreateUser(new UserEntity(user));
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateUser(UpdateUserRequestDto request, Guid currentUserId)
    {
        if (request.Id == Guid.Empty)
            throw new ValidationException("Invalid user ID");
        if (request.Id != currentUserId)
            throw new ForbiddenException();

        try
        {
            var existing = await _usersDataController.GetUserById(request.Id);
            if (existing == null)
                throw new NotFoundException("User was not found");

            var passwordHash = !string.IsNullOrWhiteSpace(request.Password)
                ? _passwordHelperService.HashPassword(request.Id.ToString(), request.Password)
                : existing.Password;

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ValidationException("User password is missing");

            var updatedEntity = new UserEntity
            {
                Id = request.Id,
                UserName = request.UserName,
                Email = request.Email,
                UserRole = existing.UserRole,
                Password = passwordHash
            };

            return await _usersDataController.UpdateUser(updatedEntity);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> ChangeUserRole(Guid userId, UserRole newRole)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            var existing = await _usersDataController.GetUserById(userId);
            if (existing == null)
                throw new NotFoundException("User was not found");

            existing.UserRole = newRole;
            return await _usersDataController.UpdateUser(existing);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid user ID");

        try
        {
            var existing = await _usersDataController.GetUserById(userId);
            if (existing == null)
                throw new NotFoundException("User was not found");

            return await _usersDataController.DeleteUser(userId);
        }
        catch (ServiceException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
}