namespace TodoList.Api.Dtos;

public record TodoListDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }

    public required List<TodoItemDto> TodoItems { get; set; }
}
