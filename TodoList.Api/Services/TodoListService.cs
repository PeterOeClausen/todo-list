using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Infrastructure.Persistance;
using TodoList.Api.Dtos;
using TodoList.Api.Models.Entities;

namespace TodoList.Api.Services;

public interface ITodoListService
{
    Task<List<TodoListDto>> GetAllAsync();
    Task<TodoListDto?> GetByIdAsync(Guid id);
    Task<TodoListDto> AddAsync(TodoListCreateDto todoList);
    Task<Tuple<bool, TodoListDto?>> UpdateAsync(Guid id, TodoListUpdateDto todoListUpdate);
    Task<bool> DeleteByIdAsync(Guid id);
}

public class TodoListService : ITodoListService
{
    public TodoListService(TodoListDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private TodoListDbContext _dbContext { get; init; }

    public async Task<List<TodoListDto>> GetAllAsync()
    {
        return await _dbContext
            .TodoLists
            .ProjectToType<TodoListDto>()
            .ToListAsync();
    }

    public async Task<TodoListDto?> GetByIdAsync(Guid id)
    {
        return await _dbContext
            .TodoLists
            .Include(x => x.TodoItems)
            .ProjectToType<TodoListDto>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TodoListDto> AddAsync(TodoListCreateDto todoList)
    {
        TodoListEntity todoListResult = todoList.Adapt<TodoListEntity>();
        await _dbContext
            .TodoLists
            .AddAsync(todoListResult);
        await _dbContext.SaveChangesAsync();
        return todoListResult.Adapt<TodoListDto>();
    }

    public async Task<Tuple<bool, TodoListDto?>> UpdateAsync(Guid id, TodoListUpdateDto todoListUpdate)
    {
        var todoList = await _dbContext
            .TodoLists
            .FirstOrDefaultAsync(x => x.Id == id);
        if (todoList is null)
        {
            return new Tuple<bool, TodoListDto?>(false, null);
        }
        todoList.Name = todoListUpdate.Name;
        await _dbContext.SaveChangesAsync();
        return new Tuple<bool, TodoListDto?>(true, todoList.Adapt<TodoListDto>());
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var todoList = await _dbContext
            .TodoLists
            .FirstOrDefaultAsync(x => x.Id == id);
        if (todoList is null)
        {
            return false;
        }
        _dbContext
            .TodoLists
            .Remove(todoList);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
