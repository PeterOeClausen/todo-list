using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Dtos;
using TodoList.Api.Infrastructure.Persistance;

namespace TodoList.Api.Commands.TodoLists;

public class UpdateTodoList(TodoListDbContext db)
{
    public async Task<(bool Success, TodoListDto? Result)> ExecuteAsync(Guid id, TodoListUpdateDto dto)
    {
        var entity = await db.TodoLists.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return (false, null);

        entity.Name = dto.Name;
        await db.SaveChangesAsync();
        return (true, entity.Adapt<TodoListDto>());
    }
}
