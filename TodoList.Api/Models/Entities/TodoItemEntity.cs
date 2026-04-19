using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoList.Api.Models.Entities;

/// <summary>
/// Repressents a TodoItem within a TodoList. Has reference to the parent TodoList via the TodoListId foreign key, and TodoList navigational property.
/// </summary>
public class TodoItemEntity
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public bool Checked { get; set; }

    public Guid TodoListId { get; set; }
    public virtual TodoListEntity TodoList { get; set; }
}

/// <summary>
/// Configuration for the TodoItemEntity in the database.
/// </summary>
public class TodoItemEntityConfiguration : IEntityTypeConfiguration<TodoItemEntity>
{
    public void Configure(EntityTypeBuilder<TodoItemEntity> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasOne(x => x.TodoList)
            .WithMany()
            .HasForeignKey(x => x.TodoListId);

        builder
            .Property(x => x.Title)
            .IsRequired();
    }
}