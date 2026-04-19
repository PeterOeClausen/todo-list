namespace TodoList.Api.Models.Dtos;

public class TodoListDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }

    public required List<TodoItemDto> TodoItems { get; set; }
}
