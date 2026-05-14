using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Dtos;
using TodoList.Api.Infrastructure.Persistance;

namespace TodoList.Api.Commands.TodoItems;

public class UpdateTodoItem(TodoListDbContext db)
{
    public async Task<(bool Success, TodoItemDto? Result)> ExecuteAsync(Guid id, TodoItemUpdateDto dto)
    {
        var entity = await db.TodoItems.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return (false, null);

        entity.Title = dto.Title;
        entity.Checked = dto.Checked;
        await db.SaveChangesAsync();
        return (true, entity.Adapt<TodoItemDto>());
    }
}
