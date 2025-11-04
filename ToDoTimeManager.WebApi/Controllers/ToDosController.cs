using Microsoft.AspNetCore.Mvc;
using ToDoTimeManager.Shared.Models;
using ToDoTimeManager.WebApi.Services.Interfaces;

namespace ToDoTimeManager.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDosController : ControllerBase
    {
        private readonly ILogger<ToDosController> _logger;
        private readonly IToDosService _toDosService;

        public ToDosController(ILogger<ToDosController> logger, IToDosService toDosService)
        {
            _logger = logger;
            _toDosService = toDosService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllToDos()
        {
            var toDos = await _toDosService.GetAllToDos();
            return Ok(toDos);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetToDoById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid to-do ID");
            var toDo = await _toDosService.GetToDoById(id);
            return toDo == null ? NotFound("To-do was not found") : Ok(toDo);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateToDo([FromBody] ToDo? toDo)
        {
            if (toDo is null)
                return BadRequest("To-do was null");
            if (toDo.Id == Guid.Empty || string.IsNullOrWhiteSpace(toDo.Title))
                return BadRequest("To-do has invalid data");
            var newToDo = await _toDosService.CreateToDo(toDo);
            return newToDo ? Ok(newToDo) : BadRequest("To-do could not be created");
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateToDo([FromBody] ToDo? toDo)
        {
            if (toDo is null)
                return BadRequest("To-do was null");
            if (toDo.Id == Guid.Empty || string.IsNullOrWhiteSpace(toDo.Title))
                return BadRequest("To-do has invalid data");
            var updatedToDo = await _toDosService.UpdateToDo(toDo);
            return updatedToDo ? Ok(updatedToDo) : BadRequest("To-do could not be updated");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteToDo(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid to-do ID");
            var deletedToDo = await _toDosService.DeleteToDo(id);
            return deletedToDo ? Ok(deletedToDo) : BadRequest("To-do could not be deleted");
        }
    }
}
