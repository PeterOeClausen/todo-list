using Microsoft.EntityFrameworkCore;
using TodoList.Api.Models.Entities;

namespace TodoList.Api.Infrastructure.Persistance;

/// <summary>
/// EF DbContext for TodoList API.
/// </summary>
public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging();
    }

    public DbSet<TodoListEntity> TodoLists { get; set; }
    public DbSet<TodoItemEntity> TodoItems { get; set; }
}
