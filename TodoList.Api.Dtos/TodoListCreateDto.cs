namespace TodoList.Api.Dtos;

public record TodoListCreateDto
{
    public required string Name { get; set; }
}
