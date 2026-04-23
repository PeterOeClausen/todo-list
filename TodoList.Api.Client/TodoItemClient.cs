using System.Net.Http.Json;
using TodoList.Api.Dtos;

namespace TodoList.Api.Client
{
    public class TodoItemClient
    {
        private const string RoutePrefix = "TodoItem";
        private readonly HttpClient _httpClient;

        public TodoItemClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TodoItemDto?> PostTodoItemAsync(TodoItemCreateDto todoItem, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(RoutePrefix, todoItem, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TodoItemDto?>(cancellationToken);
            }
            return null;
        }

        public async Task<TodoItemDto?> PutTodoItemAsync(Guid id, TodoItemUpdateDto todoItemUpdate, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{RoutePrefix}/{id}", todoItemUpdate, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TodoItemDto?>(cancellationToken);
            }
            return null;
        }

        public async Task<bool> DeleteTodoItemAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{RoutePrefix}/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
