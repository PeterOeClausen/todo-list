using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Infrastructure.Persistance;
using TodoList.Api.Dtos;
using TodoList.Api.Models.Entities;

namespace TodoList.Api.Services;

public interface ITodoItemService
{
    Task<Tuple<bool, TodoItemDto?>> AddAsync(TodoItemCreateDto todoItem);
    Task<Tuple<bool, TodoItemDto?>> UpdateAsync(Guid id, TodoItemUpdateDto todoItemUpdate);
    Task<bool> DeleteByIdAsync(Guid id);
}

public class TodoItemService : ITodoItemService
{
    public TodoItemService(TodoListDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private TodoListDbContext _dbContext { get; init; }

    public async Task<Tuple<bool, TodoItemDto?>> AddAsync(TodoItemCreateDto todoItem)
    {
        var todoList = await _dbContext
            .TodoLists
            .Where(x => x.Id == todoItem.TodoListId)
            .FirstOrDefaultAsync();
        if(todoList == null)
        {
            return new Tuple<bool, TodoItemDto?>(false, null);
        }
        var todoItemResult = todoItem.Adapt<TodoItemEntity>();
        await _dbContext
            .TodoItems
            .AddAsync(todoItemResult);
        await _dbContext.SaveChangesAsync();
        return new Tuple<bool, TodoItemDto?>(true, todoItemResult.Adapt<TodoItemDto>());
    }

    public async Task<Tuple<bool, TodoItemDto?>> UpdateAsync(Guid id, TodoItemUpdateDto todoItemUpdate)
    {
        var todoItem = _dbContext
            .TodoItems
            .Where(x => x.Id == id)
            .FirstOrDefault();
        if (todoItem == null)
        {
            return new Tuple<bool, TodoItemDto?>(false, null);
        }
        todoItem.Title = todoItemUpdate.Title;
        todoItem.Checked = todoItemUpdate.Checked;
        await _dbContext.SaveChangesAsync();
        return new Tuple<bool, TodoItemDto?>(true, todoItem.Adapt<TodoItemDto>());
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var todoItem = _dbContext
            .TodoItems
            .Where(x => x.Id == id)
            .FirstOrDefault();
        if (todoItem == null)
        {
            return false;
        }
        _dbContext.TodoItems.Remove(todoItem);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
