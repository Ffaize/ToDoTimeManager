using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

/// <summary>
/// Manages user accounts including registration, profile updates, role management, and deletion.
/// Individual endpoints carry their own authorization requirements.
/// </summary>
public class UsersController : BaseController
{
    private readonly IUsersService _usersService;

    /// <summary>
    /// Initializes a new instance of <see cref="UsersController"/>.
    /// </summary>
    /// <param name="usersService">The service used to perform user account operations.</param>
    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    /// <summary>
    /// Retrieves all registered user accounts. Restricted to administrators.
    /// Sensitive fields such as passwords are excluded from the response.
    /// </summary>
    /// <returns>200 OK with a list of <see cref="UserResponseDto"/> objects.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        List<User> users = await _usersService.GetAllUsers();
        return Ok(users.Select(ToUserResponseDto));
    }

    /// <summary>
    /// Retrieves a user account by its unique identifier.
    /// Administrators may access any account; regular users may only access their own profile.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>
    /// 200 OK with a <see cref="UserResponseDto"/> on success;
    /// 500 Internal Server Error if the user is not found or the caller lacks access.
    /// </returns>
    [Authorize]
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _usersService.GetUserById(id, GetCurrentUserId(), IsAdmin());
        return user != null ? Ok(ToUserResponseDto(user)) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves a user account by their username. Restricted to administrators.
    /// </summary>
    /// <param name="userName">The exact username to search for.</param>
    /// <returns>
    /// 200 OK with a <see cref="UserResponseDto"/> on success;
    /// 500 Internal Server Error if no matching user is found.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("GetByUsername/{userName}")]
    public async Task<IActionResult> GetUserByUsername(string userName)
    {
        var user = await _usersService.GetUserByUsername(userName);
        return user != null ? Ok(ToUserResponseDto(user)) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves a user account by their email address. Restricted to administrators.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>
    /// 200 OK with a <see cref="UserResponseDto"/> on success;
    /// 500 Internal Server Error if no matching user is found.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("GetByEmail/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _usersService.GetUserByEmail(email);
        return user != null ? Ok(ToUserResponseDto(user)) : StatusCode(500);
    }

    /// <summary>
    /// Retrieves a user account by a login parameter that may be either a username or email address.
    /// Restricted to administrators.
    /// </summary>
    /// <param name="loginParameter">The username or email address to search for.</param>
    /// <returns>
    /// 200 OK with a <see cref="UserResponseDto"/> on success;
    /// 500 Internal Server Error if no matching user is found.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("GetByLoginParameter/{loginParameter}")]
    public async Task<IActionResult> GetUserByLoginParameter(string loginParameter)
    {
        var user = await _usersService.GetUserByLoginParameter(loginParameter);
        return user != null ? Ok(ToUserResponseDto(user)) : StatusCode(500);
    }

    /// <summary>
    /// Registers a new user account. Publicly accessible without authentication.
    /// </summary>
    /// <param name="request">
    /// The registration payload containing username, email, password, and the desired role.
    /// </param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if registration fails.
    /// </returns>
    [AllowAnonymous]
    [HttpPost("Create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
    {
        var created = await _usersService.CreateUser(request);
        return created ? Ok(created) : StatusCode(500);
    }

    /// <summary>
    /// Updates the profile of the currently authenticated user.
    /// Users may only update their own profile.
    /// </summary>
    /// <param name="request">The update payload containing the new username, email, and optional password.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the update fails.
    /// </returns>
    [Authorize]
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDto request)
    {
        var result = await _usersService.UpdateUser(request, GetCurrentUserId());
        return result ? Ok(result) : StatusCode(500);
    }

    /// <summary>
    /// Changes the role assigned to a user account. Restricted to administrators.
    /// </summary>
    /// <param name="id">The unique identifier of the user whose role is to be changed.</param>
    /// <param name="request">The payload specifying the new <see cref="UserRole"/> to assign.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if the operation fails.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("ChangeRole/{id}")]
    public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeUserRoleRequestDto request)
    {
        var changed = await _usersService.ChangeUserRole(id, request.NewRole);
        return changed ? Ok(changed) : StatusCode(500);
    }

    /// <summary>
    /// Permanently deletes a user account by its unique identifier. Restricted to administrators.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>
    /// 200 OK with <c>true</c> on success;
    /// 500 Internal Server Error if deletion fails.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var deleted = await _usersService.DeleteUser(id);
        return deleted ? Ok(deleted) : StatusCode(500);
    }

    private static UserResponseDto ToUserResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            UserRole = user.UserRole
        };
    }
}