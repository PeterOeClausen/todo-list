namespace TodoList.Api.Models.Dtos;

public class TodoItemUpdateDto
{
    public required string Title { get; set; }
    public required bool Checked { get; set; }
}
