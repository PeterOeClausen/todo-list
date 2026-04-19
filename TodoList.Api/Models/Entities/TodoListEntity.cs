using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoList.Api.Models.Entities;

/// <summary>
/// Repressents a TodoList in the database. A TodoList can have multiple TodoItems.
/// </summary>
public class TodoListEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public virtual ICollection<TodoItemEntity> TodoItems { get; set; } = new List<TodoItemEntity>();
}

public class TodoListEntityConfiguration : IEntityTypeConfiguration<TodoListEntity>
{
    public void Configure(EntityTypeBuilder<TodoListEntity> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Name)
            .IsRequired();
    }
}