
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Models.Dtos;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoListController : ControllerBase
{
    public TodoListController()
    {
        Guid todoListId = Guid.NewGuid();
        todoLists = new List<TodoListDto>()
        {
            new TodoListDto()
            {
                Id = todoListId,
                Name = "Creating TodoList API",
                TodoItems = new List<TodoItemDto>()
                {
                    new TodoItemDto()
                    {
                        Id = Guid.NewGuid(),
                        Title = "Create API endpoints",
                        Checked = false,
                        TodoListId = todoListId
                    }
                }
            }
        };
    }
    private List<TodoListDto> todoLists { get; set; }

    [HttpGet]
    public IActionResult GetTodoLists()
    {
        return Ok(todoLists);
    }
}
