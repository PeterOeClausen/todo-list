namespace TodoList.Api.Dtos;

public record TodoItemCreateDto
{
    public required string Title { get; set; }
    public required bool Checked { get; set; }
    public required Guid TodoListId { get; set; }
}