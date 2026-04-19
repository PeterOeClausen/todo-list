using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Infrastructure.Persistance;
using TodoList.Api.Models.Dtos;
using TodoList.Api.Models.Entities;

namespace TodoList.Api.Services;

public interface ITodoList
{
    Task<List<TodoListDto>> GetAllAsync();
    Task<TodoListDto?> GetById(Guid id);
    Task<TodoListDto> AddAsync(TodoListCreateDto todoList);
    Task<bool> DeleteById(Guid id);
}

public class TodoListService : ITodoList
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

    public async Task<TodoListDto?> GetById(Guid id)
    {
        return await _dbContext
            .TodoLists
            .ProjectToType<TodoListDto>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TodoListDto> AddAsync(TodoListCreateDto todoList)
    {
        TodoListEntity todoListEntity = todoList.Adapt<TodoListEntity>();
        await _dbContext
            .TodoLists
            .AddAsync(todoListEntity);
        await _dbContext.SaveChangesAsync();
        return todoList.Adapt<TodoListDto>();
    }

    public async Task<bool> DeleteById(Guid id)
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
