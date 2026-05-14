using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Dtos;
using TodoList.Api.Infrastructure.Persistance;
using TodoList.Api.Models.Entities;

namespace TodoList.Api.Commands.TodoItems;

public class CreateTodoItem(TodoListDbContext db)
{
    public async Task<(bool Success, TodoItemDto? Result)> ExecuteAsync(TodoItemCreateDto dto)
    {
        var listExists = await db.TodoLists.AnyAsync(x => x.Id == dto.TodoListId);
        if (!listExists)
            return (false, null);

        var entity = dto.Adapt<TodoItemEntity>();
        await db.TodoItems.AddAsync(entity);
        await db.SaveChangesAsync();
        return (true, entity.Adapt<TodoItemDto>());
    }
}
