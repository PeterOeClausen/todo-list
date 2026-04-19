using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Models.Dtos;
using TodoList.Api.Services;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoListController : ControllerBase
{
    public TodoListController(TodoListService todoListService)
    {
        _todoListService = todoListService;
    }

    private TodoListService _todoListService { get; init; }

    [HttpGet]
    public async Task<ActionResult<List<TodoListDto>>> GetTodoLists()
    {
        return Ok(await _todoListService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetTodoListById([FromRoute] Guid id)
    {
        var todoList = await _todoListService.GetById(id);
        if(todoList is null)
        {
            return NotFound($"No TodoList with Id '{id}' were found.");
        }
        return Ok(todoList);
    }

    [HttpPost]
    public async Task<ActionResult<TodoListDto>> CreateTodoList([FromBody] TodoListCreateDto todoListDto)
    {
        var createdTodoList = await _todoListService.AddAsync(todoListDto);
        return CreatedAtAction(nameof(GetTodoListById), new { id = createdTodoList.Id }, createdTodoList);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoList([FromRoute] Guid id)
    {
        var deleted = await _todoListService.DeleteById(id);
        if (!deleted)
        {
            return NotFound($"No TodoList with Id '{id}' were found.");
        }
        return NoContent();
    }
}
