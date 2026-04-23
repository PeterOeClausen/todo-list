using System.Net;
using TodoList.Api.Client;
using TodoList.Api.Dtos;
using TodoList.Api.Tests.Infrastructure;

namespace TodoList.Api.Tests;

[Collection(IntegrationCollection.Name)]
public class TodoItemControllerTests : IAsyncLifetime
{
    private readonly IntegrationFixture _fixture;
    private readonly TodoListApiClient _apiClient;

    public TodoItemControllerTests(IntegrationFixture fixture)
    {
        _fixture = fixture;
        _apiClient = new TodoListApiClient(_fixture.Factory.CreateClient());
    }

    public Task InitializeAsync() => _fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<TodoListDto> CreateTestTodoListAsync(string name = "Parent list")
    {
        var result = await _apiClient.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = name }, CancellationToken.None);
        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.NotNull(result.Value);
        return result.Value;
    }

    [Fact]
    public async Task PostTodoItem_AddsItemToList_WhenParentExists()
    {
        // Arrange
        var list = await CreateTestTodoListAsync();

        // Act
        var result = await _apiClient.TodoItems.PostTodoItemAsync(
            new TodoItemCreateDto
            {
                Title = "Buy milk",
                Checked = false,
                TodoListId = list.Id,
            },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.NotNull(result.Value);
        var created = result.Value;
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Buy milk", created.Title);
        Assert.False(created.Checked);
        Assert.Equal(list.Id, created.TodoListId);

        // Verify persistence by re-reading the parent list and checking it has the new item.
        var fetchedList = await _apiClient.TodoLists.GetTodoListByIdAsync(list.Id, CancellationToken.None);
        Assert.True(fetchedList.IsSuccess);
        Assert.NotNull(fetchedList.Value);
        Assert.Single(fetchedList.Value.TodoItems);
        Assert.Equal("Buy milk", fetchedList.Value.TodoItems.Single().Title);
    }

    [Fact]
    public async Task PostTodoItem_Returns404_WhenParentListMissing()
    {
        // Act
        var result = await _apiClient.TodoItems.PostTodoItemAsync(
            new TodoItemCreateDto
            {
                Title = "Orphan",
                Checked = false,
                TodoListId = Guid.NewGuid(),
            },
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("No TodoList with Id", result.ErrorMessage);
    }

    [Fact]
    public async Task PutTodoItem_UpdatesTitleAndChecked_WhenExists()
    {
        // Arrange
        var list = await CreateTestTodoListAsync();
        var createResult = await _apiClient.TodoItems.PostTodoItemAsync(
            new TodoItemCreateDto { Title = "Original", Checked = false, TodoListId = list.Id },
            CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        var item = createResult.Value!;

        // Act
        var updateResult = await _apiClient.TodoItems.PutTodoItemAsync(
            item.Id,
            new TodoItemUpdateDto { Title = "Updated", Checked = true },
            CancellationToken.None);

        // Assert
        Assert.True(updateResult.IsSuccess, updateResult.ErrorMessage);
        Assert.NotNull(updateResult.Value);
        var updated = updateResult.Value;
        Assert.Equal(item.Id, updated.Id);
        Assert.Equal("Updated", updated.Title);
        Assert.True(updated.Checked);

        // Verify persistence by re-reading the parent list.
        var fetchedList = await _apiClient.TodoLists.GetTodoListByIdAsync(list.Id, CancellationToken.None);
        Assert.True(fetchedList.IsSuccess);
        var fetchedItem = Assert.Single(fetchedList.Value!.TodoItems);
        Assert.Equal("Updated", fetchedItem.Title);
        Assert.True(fetchedItem.Checked);
    }

    [Fact]
    public async Task PutTodoItem_Returns404_WhenMissing()
    {
        // Act
        var result = await _apiClient.TodoItems.PutTodoItemAsync(
            Guid.NewGuid(),
            new TodoItemUpdateDto { Title = "x", Checked = false },
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeleteTodoItem_Returns204_AndRemovesIt_WhenExists()
    {
        // Arrange
        var first = await CreateTestTodoListAsync();
        var createResult = await _apiClient.TodoItems.PostTodoItemAsync(
            new TodoItemCreateDto { Title = "Delete me", Checked = false, TodoListId = first.Id },
            CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        var second = createResult.Value!;

        // Act
        var deleteResult = await _apiClient.TodoItems.DeleteTodoItemAsync(second.Id, CancellationToken.None);

        // Assert
        Assert.True(deleteResult.IsSuccess, deleteResult.ErrorMessage);
        Assert.Equal((int)HttpStatusCode.NoContent, deleteResult.StatusCode);

        // Verify the parent list still exists, but with no items.
        var fetchedList = await _apiClient.TodoLists.GetTodoListByIdAsync(first.Id, CancellationToken.None);
        Assert.True(fetchedList.IsSuccess);
        Assert.NotNull(fetchedList.Value);
        Assert.Empty(fetchedList.Value.TodoItems);
    }

    [Fact]
    public async Task DeleteTodoItem_Returns404_WhenMissing()
    {
        // Act
        var result = await _apiClient.TodoItems.DeleteTodoItemAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeletingParentList_CascadeDeletesItems()
    {
        // Arrange — TodoItemEntity.TodoListId is a non-nullable FK with no explicit OnDelete
        // configured, so EF Core's convention is DeleteBehavior.Cascade. Deleting the parent
        // should succeed and remove its items.
        var list = await CreateTestTodoListAsync();
        var createResult = await _apiClient.TodoItems.PostTodoItemAsync(
            new TodoItemCreateDto { Title = "Child", Checked = false, TodoListId = list.Id },
            CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        var item = createResult.Value!;

        // Act
        var deleteResult = await _apiClient.TodoLists.DeleteTodoListAsync(list.Id, CancellationToken.None);

        // Assert
        Assert.True(deleteResult.IsSuccess, deleteResult.ErrorMessage);

        // The list is gone.
        var listResult = await _apiClient.TodoLists.GetTodoListByIdAsync(list.Id, CancellationToken.None);
        Assert.False(listResult.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, listResult.StatusCode);

        // And its child item should also be gone (deleting the item again returns 404).
        var deleteItemAgain = await _apiClient.TodoItems.DeleteTodoItemAsync(item.Id, CancellationToken.None);
        Assert.False(deleteItemAgain.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, deleteItemAgain.StatusCode);
    }
}
