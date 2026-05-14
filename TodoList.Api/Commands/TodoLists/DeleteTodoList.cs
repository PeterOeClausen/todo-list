using Microsoft.EntityFrameworkCore;
using TodoList.Api.Infrastructure.Persistance;

namespace TodoList.Api.Commands.TodoLists;

public class DeleteTodoList(TodoListDbContext db)
{
    public async Task<bool> ExecuteAsync(Guid id)
    {
        var entity = await db.TodoLists.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return false;

        db.TodoLists.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}
