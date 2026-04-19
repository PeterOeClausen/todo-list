namespace TodoList.Api.Models.Dtos;

public record TodoListCreateDto
{
    public required string Name { get; set; }
}
