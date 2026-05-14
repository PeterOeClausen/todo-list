using Mapster;
using TodoList.Api.Dtos;
using TodoList.Api.Infrastructure.Persistance;
using TodoList.Api.Models.Entities;

namespace TodoList.Api.Commands.TodoLists;

public class CreateTodoList(TodoListDbContext db)
{
    public async Task<TodoListDto> ExecuteAsync(TodoListCreateDto dto)
    {
        var entity = dto.Adapt<TodoListEntity>();
        await db.TodoLists.AddAsync(entity);
        await db.SaveChangesAsync();
        return entity.Adapt<TodoListDto>();
    }
}
