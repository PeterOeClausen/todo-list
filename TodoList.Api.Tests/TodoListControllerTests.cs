using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoList.Api.Client;
using TodoList.Api.Dtos;

namespace TodoList.Api.Tests
{
    public class TodoListControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TodoListApiClient _todoListApiClient;

        public TodoListControllerTests(WebApplicationFactory<Program> factory)
        {
            // Spin up API in memory
            _client = factory.CreateClient();
            _todoListApiClient = new TodoListApiClient(_client);
        }

        [Fact]
        public async Task GetTodoLists()
        {
            // Act
            List<TodoListDto>? response = await _todoListApiClient.TodoListClient.GetTodoListsAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(response);
        }
    }
}
