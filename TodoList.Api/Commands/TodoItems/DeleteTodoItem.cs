using Microsoft.EntityFrameworkCore;
using TodoList.Api.Infrastructure.Persistance;

namespace TodoList.Api.Commands.TodoItems;

public class DeleteTodoItem(TodoListDbContext db)
{
    public async Task<bool> ExecuteAsync(Guid id)
    {
        var entity = await db.TodoItems.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
            return false;

        db.TodoItems.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}
