using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Models.Dtos;
using TodoList.Api.Services;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoListController : ControllerBase
{
    public TodoListController(ITodoListService todoListService)
    {
        _todoListService = todoListService;
    }

    private ITodoListService _todoListService { get; init; }

    [HttpGet]
    public async Task<ActionResult<List<TodoListDto>>> GetTodoLists()
    {
        return Ok(await _todoListService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetTodoListById([FromRoute] Guid id)
    {
        var todoList = await _todoListService.GetByIdAsync(id);
        if(todoList is null)
        {
            return NotFound($"No TodoList with Id '{id}' were found.");
        }
        return Ok(todoList);
    }

    [HttpPost]
    public async Task<ActionResult<TodoListDto>> PostTodoList([FromBody] TodoListCreateDto todoListDto)
    {
        var createdTodoList = await _todoListService.AddAsync(todoListDto);
        return CreatedAtAction(nameof(GetTodoListById), new { id = createdTodoList.Id }, createdTodoList);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoListDto>> PutTodoList([FromRoute] Guid id, [FromBody] TodoListUpdateDto todoListUpdate)
    {
        var updateResult = await _todoListService.UpdateAsync(id, todoListUpdate);
        if (!updateResult.Item1)
        {
            return NotFound($"No TodoList with Id '{id}' were found.");
        }
        return Ok(updateResult.Item2);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoList([FromRoute] Guid id)
    {
        var deleted = await _todoListService.DeleteByIdAsync(id);
        if (!deleted)
        {
            return NotFound($"No TodoList with Id '{id}' were found.");
        }
        return NoContent();
    }
}
