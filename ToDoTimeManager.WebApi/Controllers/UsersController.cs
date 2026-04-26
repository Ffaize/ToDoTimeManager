using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoTimeManager.Shared.DTOs;
using ToDoTimeManager.Shared.Enums;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
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
        var user = await _usersService.GetUserById(id, GetCurrentUserId(), IsAdmin());
        return user != null ? Ok(ToUserResponseDto(user)) : StatusCode(500);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetByUsername/{userName}")]
    public async Task<IActionResult> GetUserByUsername(string userName)
    {
        var user = await _usersService.GetUserByUsername(userName);
        return user != null ? Ok(ToUserResponseDto(user)) : StatusCode(500);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetByEmail/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _usersService.GetUserByEmail(email);
        return user != null ? Ok(ToUserResponseDto(user)) : StatusCode(500);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetByLoginParameter/{loginParameter}")]
    public async Task<IActionResult> GetUserByLoginParameter(string loginParameter)
    {
        var user = await _usersService.GetUserByLoginParameter(loginParameter);
        return user != null ? Ok(ToUserResponseDto(user)) : StatusCode(500);
    }

    [AllowAnonymous]
    [HttpPost("Create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
    {
        var created = await _usersService.CreateUser(request);
        return created ? Ok(created) : StatusCode(500);
    }

    [Authorize]
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDto request)
    {
        var result = await _usersService.UpdateUser(request, GetCurrentUserId());
        return result ? Ok(result) : StatusCode(500);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("ChangeRole/{id}")]
    public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeUserRoleRequestDto request)
    {
        var changed = await _usersService.ChangeUserRole(id, request.NewRole);
        return changed ? Ok(changed) : StatusCode(500);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var deleted = await _usersService.DeleteUser(id);
        return deleted ? Ok(deleted) : StatusCode(500);
    }

    private static UserResponseDto ToUserResponseDto(User user) => new()
    {
        Id       = user.Id,
        UserName = user.UserName,
        Email    = user.Email,
        UserRole = user.UserRole
    };
}
