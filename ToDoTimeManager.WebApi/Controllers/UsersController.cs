using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;
using ToDoTimeManager.WebApi.Utils.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUsersService _usersService;
    private readonly IPasswordHelperService _passwordHelperService;

    public UsersController(ILogger<UsersController> logger, IUsersService usersService, IPasswordHelperService passwordHelperService)
    {
        _logger = logger;
        _usersService = usersService;
        _passwordHelperService = passwordHelperService;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("Admin");

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _usersService.GetAllUsers();
        return Ok(users.Select(ToUserResponseDto));
    }

    [Authorize]
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid user ID");
        if (id != GetCurrentUserId() && !IsAdmin())
            return Forbid();
        var user = await _usersService.GetUserById(id);
        return user == null ? NotFound("User was not found") : Ok(ToUserResponseDto(user));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetByUsername/{userName}")]
    public async Task<IActionResult> GetUserByUsername(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return BadRequest("Invalid username");
        var user = await _usersService.GetUserByUsername(userName);
        return user is null ? NotFound("User was not found") : Ok(ToUserResponseDto(user));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetByEmail/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Invalid email");
        var user = await _usersService.GetUserByEmail(email);
        return user is null ? NotFound("User was not found") : Ok(ToUserResponseDto(user));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetByLoginParameter/{loginParameter}")]
    public async Task<IActionResult> GetUserByLoginParameter(string loginParameter)
    {
        if (string.IsNullOrWhiteSpace(loginParameter))
            return BadRequest("Invalid login parameter");
        var user = await _usersService.GetUserByLoginParameter(loginParameter);
        return user is null ? NotFound("User was not found") : Ok(ToUserResponseDto(user));
    }

    [AllowAnonymous]
    [HttpPost("Create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
    {
        var user = new User
        {
            Id = request.Id,
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password,
            UserRole = UserRole.User  // role is always set server-side; client cannot escalate to Admin
        };

        var created = await _usersService.CreateUser(user);
        return created ? Ok(created) : BadRequest("User could not be created");
    }

    [Authorize]
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDto request)
    {
        if (request.Id != GetCurrentUserId())
            return Forbid();

        var existingUser = await _usersService.GetUserById(request.Id);
        if (existingUser is null)
            return NotFound("User was not found");

        var passwordHash = !string.IsNullOrWhiteSpace(request.Password)
            ? _passwordHelperService.HashPassword(request.Id.ToString(), request.Password)
            : existingUser.Password;

        if (string.IsNullOrWhiteSpace(passwordHash))
            return BadRequest("User password is missing");

        var updatedUser = new User
        {
            Id = request.Id,
            UserName = request.UserName,
            Email = request.Email,
            UserRole = existingUser.UserRole,  // role is never changed via this endpoint
            Password = passwordHash
        };

        var res = await _usersService.UpdateUser(updatedUser);
        return res ? Ok(res) : BadRequest("User could not be updated");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("ChangeRole/{id}")]
    public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeUserRoleRequestDto request)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid user ID");
        var changed = await _usersService.ChangeUserRole(id, request.NewRole);
        return changed ? Ok(changed) : NotFound("User was not found");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid user ID");
        var deletedUser = await _usersService.DeleteUser(id);
        return deletedUser ? Ok(deletedUser) : BadRequest("User could not be deleted");
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
