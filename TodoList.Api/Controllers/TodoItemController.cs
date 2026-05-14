using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Commands.TodoItems;
using TodoList.Api.Dtos;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoItemController(
    CreateTodoItem createTodoItem,
    UpdateTodoItem updateTodoItem,
    DeleteTodoItem deleteTodoItem) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> AddTodoItem([FromBody] TodoItemCreateDto dto)
    {
        var (success, result) = await createTodoItem.ExecuteAsync(dto);
        if (!success)
            return NotFound($"No TodoList with Id '{dto.TodoListId}' were found.");
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTodoItem([FromRoute] Guid id, [FromBody] TodoItemUpdateDto dto)
    {
        var (success, result) = await updateTodoItem.ExecuteAsync(id, dto);
        if (!success)
            return NotFound($"No TodoItem with Id '{id}' were found.");
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoItem([FromRoute] Guid id)
    {
        var deleted = await deleteTodoItem.ExecuteAsync(id);
        if (!deleted)
            return NotFound($"No TodoItem with Id '{id}' were found.");
        return NoContent();
    }
}
