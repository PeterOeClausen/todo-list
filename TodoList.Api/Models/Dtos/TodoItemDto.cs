namespace TodoList.Api.Models.Dtos;

public record TodoItemDto : TodoItemCreateDto
{
    public required Guid Id { get; set; }
}
