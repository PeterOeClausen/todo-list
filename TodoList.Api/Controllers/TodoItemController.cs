using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Models.Dtos;
using TodoList.Api.Services;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoItemController : ControllerBase
{
    public TodoItemController(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    private ITodoItemService _todoItemService { get; init; }

    [HttpPost]
    public async Task<ActionResult> AddTodoItem([FromBody] TodoItemCreateDto todoItem)
    {
        var addResult = await _todoItemService.AddAsync(todoItem);
        if (!addResult.Item1)
        {
            return NotFound($"No TodoList with Id '{todoItem.TodoListId}' were found.");
        }
        return Ok(addResult.Item2);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTodoItem([FromRoute] Guid id, [FromBody] TodoItemUpdateDto todoItemUpdate)
    {
        var updateResult = await _todoItemService.UpdateAsync(id, todoItemUpdate);
        if (!updateResult.Item1)
        {
            return NotFound($"No TodoItem with Id '{id}' were found.");
        }
        return Ok(updateResult.Item2);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoItem([FromRoute] Guid id)
    {
        var deleted = await _todoItemService.DeleteByIdAsync(id);
        if (!deleted)
        {
            return NotFound($"No TodoItem with Id '{id}' were found.");
        }
        return NoContent();
    }
}
