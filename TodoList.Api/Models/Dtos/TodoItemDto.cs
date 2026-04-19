namespace TodoList.Api.Models.Dtos;

public class TodoItemDto : TodoItemCreateDto
{
    public required Guid Id { get; set; }
}
