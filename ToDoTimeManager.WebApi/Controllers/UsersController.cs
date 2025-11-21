using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUsersService _usersService;

    public UsersController(ILogger<UsersController> logger, IUsersService usersService)
    {
        _logger = logger;
        _usersService = usersService;
    }

    [Authorize]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _usersService.GetAllUsers();
        return Ok(users);
    }

    [Authorize]
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid user ID");
        var user = await _usersService.GetUserById(id);
        return user == null ? NotFound("User was not found") : Ok(user);
    }

    [Authorize]
    [HttpGet("GetByUsername/{userName}")]
    public async Task<IActionResult> GetUserByUsername(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return BadRequest("Invalid username");
        var user = await _usersService.GetUserByUsername(userName);
        return user is null ? NotFound("User was not found") : Ok(user);
    }

    [Authorize]
    [HttpGet("GetByEmail/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Invalid email");
        var user = await _usersService.GetUserByEmail(email);
        return user is null ? NotFound("User was not found") : Ok(user);
    }

    [Authorize]
    [HttpGet("GetByLoginParameter/{loginParameter}")]
    public async Task<IActionResult> GetUserByLoginParameter(string loginParameter)
    {
        if (string.IsNullOrWhiteSpace(loginParameter))
            return BadRequest("Invalid login parameter");
        var user = await _usersService.GetUserByLoginParameter(loginParameter);
        return user is null ? NotFound("User was not found") : Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("Create")]
    public async Task<IActionResult> CreateUser([FromBody] User? user)
    {
        if (user is null)
            return BadRequest("User was null");

        if (user.Id == Guid.Empty || user.UserRole is null ||
            string.IsNullOrWhiteSpace(user.Email) ||
            string.IsNullOrWhiteSpace(user.Password) ||
            string.IsNullOrWhiteSpace(user.UserName))
            return BadRequest("User has invalid credentials");


        var newUser = await _usersService.CreateUser(user); 

        return newUser ? Ok(newUser) : BadRequest("User could not be created");
    }

    [Authorize]
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateUser([FromBody] User? user)
    {
        if (user is null)
            return BadRequest("User was null");
        if (user.Id == Guid.Empty || user.UserRole is null ||
            string.IsNullOrWhiteSpace(user.Email) ||
            string.IsNullOrWhiteSpace(user.Password) ||
            string.IsNullOrWhiteSpace(user.UserName))
            return BadRequest("User has invalid credentials");
        var updatedUser = await _usersService.UpdateUser(user);
        return updatedUser ? Ok(updatedUser) : BadRequest("User could not be updated");
    }

    [Authorize]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid user ID");
        var deletedUser = await _usersService.DeleteUser(id);
        return deletedUser ? Ok(deletedUser) : BadRequest("User could not be deleted");
    }


}