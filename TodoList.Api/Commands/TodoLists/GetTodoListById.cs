using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Dtos;
using TodoList.Api.Infrastructure.Persistance;

namespace TodoList.Api.Commands.TodoLists;

public class GetTodoListById(TodoListDbContext db)
{
    public async Task<TodoListDto?> ExecuteAsync(Guid id)
    {
        return await db.TodoLists
            .Include(x => x.TodoItems)
            .ProjectToType<TodoListDto>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
