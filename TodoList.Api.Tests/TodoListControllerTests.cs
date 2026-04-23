using System.Net;
using System.Net.Http.Json;
using TodoList.Api.Client;
using TodoList.Api.Dtos;
using TodoList.Api.Tests.Infrastructure;

namespace TodoList.Api.Tests;

[Collection(IntegrationCollection.Name)]
public class TodoListControllerTests : IAsyncLifetime
{
    private readonly IntegrationFixture _fixture;
    private readonly HttpClient _httpClient;
    private readonly TodoListApiClient _api;

    public TodoListControllerTests(IntegrationFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.Factory.CreateClient();
        _api = new TodoListApiClient(_httpClient);
    }

    // Reset the database before every test so tests are isolated.
    public Task InitializeAsync() => _fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetTodoLists_ReturnsEmpty_WhenNoListsExist()
    {
        var result = await _api.TodoLists.GetTodoListsAsync(CancellationToken.None);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetTodoLists_ReturnsAllLists_WhenListsExist()
    {
        await _api.TodoLists.PostTodoListAsync(new TodoListCreateDto { Name = "Groceries" }, CancellationToken.None);
        await _api.TodoLists.PostTodoListAsync(new TodoListCreateDto { Name = "Chores" }, CancellationToken.None);

        var result = await _api.TodoLists.GetTodoListsAsync(CancellationToken.None);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Contains(result.Value, l => l.Name == "Groceries");
        Assert.Contains(result.Value, l => l.Name == "Chores");
    }

    [Fact]
    public async Task GetTodoListById_ReturnsList_WhenExists()
    {
        var createResult = await _api.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = "Weekend" }, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        var created = createResult.Value!;

        var result = await _api.TodoLists.GetTodoListByIdAsync(created.Id, CancellationToken.None);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(created.Id, result.Value.Id);
        Assert.Equal("Weekend", result.Value.Name);
        Assert.Empty(result.Value.TodoItems);
    }

    [Fact]
    public async Task GetTodoListById_Returns404_WhenMissing()
    {
        var result = await _api.TodoLists.GetTodoListByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("No TodoList with Id", result.ErrorMessage);
    }

    [Fact]
    public async Task PostTodoList_Returns201_WithLocationHeader()
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "/TodoList",
            new TodoListCreateDto { Name = "Travel" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        // The Location header must point at a resource we can GET.
        using var fetchByLocation = await _httpClient.GetAsync(response.Headers.Location);
        fetchByLocation.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PostTodoList_PersistsToDatabase()
    {
        var postResult = await _api.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = "Persisted" }, CancellationToken.None);
        Assert.True(postResult.IsSuccess, postResult.ErrorMessage);
        Assert.Equal((int)HttpStatusCode.Created, postResult.StatusCode);
        Assert.NotNull(postResult.Value);
        var created = postResult.Value;

        // Use a fresh client to be sure we're reading through the API/DB.
        var freshApi = new TodoListApiClient(_fixture.Factory.CreateClient());
        var fetched = await freshApi.TodoLists.GetTodoListByIdAsync(created.Id, CancellationToken.None);

        Assert.True(fetched.IsSuccess, fetched.ErrorMessage);
        Assert.NotNull(fetched.Value);
        Assert.Equal("Persisted", fetched.Value.Name);
    }

    [Fact]
    public async Task PutTodoList_UpdatesName_WhenExists()
    {
        var createResult = await _api.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = "Old name" }, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        var created = createResult.Value!;

        var update = await _api.TodoLists.PutTodoListAsync(
            created.Id,
            new TodoListUpdateDto { Name = "New name" },
            CancellationToken.None);

        Assert.True(update.IsSuccess, update.ErrorMessage);
        Assert.NotNull(update.Value);
        Assert.Equal(created.Id, update.Value.Id);
        Assert.Equal("New name", update.Value.Name);

        var fetched = await _api.TodoLists.GetTodoListByIdAsync(created.Id, CancellationToken.None);
        Assert.True(fetched.IsSuccess);
        Assert.Equal("New name", fetched.Value!.Name);
    }

    [Fact]
    public async Task PutTodoList_Returns404_WhenMissing()
    {
        var result = await _api.TodoLists.PutTodoListAsync(
            Guid.NewGuid(),
            new TodoListUpdateDto { Name = "Anything" },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeleteTodoList_Returns204_AndRemovesIt_WhenExists()
    {
        var createResult = await _api.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = "Delete me" }, CancellationToken.None);
        Assert.True(createResult.IsSuccess);
        var created = createResult.Value!;

        var deleteResult = await _api.TodoLists.DeleteTodoListAsync(created.Id, CancellationToken.None);
        Assert.True(deleteResult.IsSuccess, deleteResult.ErrorMessage);
        Assert.Equal((int)HttpStatusCode.NoContent, deleteResult.StatusCode);

        var fetchResult = await _api.TodoLists.GetTodoListByIdAsync(created.Id, CancellationToken.None);
        Assert.False(fetchResult.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, fetchResult.StatusCode);
    }

    [Fact]
    public async Task DeleteTodoList_Returns404_WhenMissing()
    {
        var result = await _api.TodoLists.DeleteTodoListAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DatabaseIsResetBetweenTests_FirstTest()
    {
        // Sanity check #1: confirms Respawn isolates this test from any state created elsewhere.
        var result = await _api.TodoLists.GetTodoListsAsync(CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);

        await _api.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = "Leftover state" }, CancellationToken.None);
    }

    [Fact]
    public async Task DatabaseIsResetBetweenTests_SecondTest()
    {
        // Sanity check #2: if Respawn weren't running, the list created above would still be here.
        var result = await _api.TodoLists.GetTodoListsAsync(CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
