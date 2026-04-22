namespace TodoList.Api.Dtos;

public record TodoListUpdateDto
{
    public required string Name { get; set; }
}
