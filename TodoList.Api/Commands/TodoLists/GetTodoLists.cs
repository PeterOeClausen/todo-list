using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Dtos;
using TodoList.Api.Infrastructure.Persistance;

namespace TodoList.Api.Commands.TodoLists;

public class GetTodoLists(TodoListDbContext db)
{
    public async Task<List<TodoListDto>> ExecuteAsync()
    {
        return await db.TodoLists
            .ProjectToType<TodoListDto>()
            .ToListAsync();
    }
}
