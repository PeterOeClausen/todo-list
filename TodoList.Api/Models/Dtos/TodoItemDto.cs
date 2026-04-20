namespace TodoList.Api.Models.Dtos;

public record TodoItemDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required bool Checked { get; set; }
    public required Guid TodoListId { get; set; }
}
