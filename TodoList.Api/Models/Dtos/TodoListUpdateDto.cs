namespace TodoList.Api.Models.Dtos;

public record TodoListUpdateDto
{
    public required string Name { get; set; }
}
