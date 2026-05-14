using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Commands.TodoLists;
using TodoList.Api.Dtos;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoListController(
    GetTodoLists getTodoLists,
    GetTodoListById getTodoListById,
    CreateTodoList createTodoList,
    UpdateTodoList updateTodoList,
    DeleteTodoList deleteTodoList) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TodoListDto>>> GetTodoLists()
    {
        return Ok(await getTodoLists.ExecuteAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetTodoListById([FromRoute] Guid id)
    {
        var todoList = await getTodoListById.ExecuteAsync(id);
        if (todoList is null)
            return NotFound($"No TodoList with Id '{id}' were found.");
        return Ok(todoList);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<TodoListDto>> PostTodoList([FromBody] TodoListCreateDto dto)
    {
        var created = await createTodoList.ExecuteAsync(dto);
        return CreatedAtAction(nameof(GetTodoListById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoListDto>> PutTodoList([FromRoute] Guid id, [FromBody] TodoListUpdateDto dto)
    {
        var (success, result) = await updateTodoList.ExecuteAsync(id, dto);
        if (!success)
            return NotFound($"No TodoList with Id '{id}' were found.");
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoList([FromRoute] Guid id)
    {
        var deleted = await deleteTodoList.ExecuteAsync(id);
        if (!deleted)
            return NotFound($"No TodoList with Id '{id}' were found.");
        return NoContent();
    }
}
