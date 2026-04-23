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

        public async Task<ApiResult<TodoItemDto>> PostTodoItemAsync(TodoItemCreateDto todoItem, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.PostAsJsonAsync(RoutePrefix, todoItem, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TodoItemDto>(cancellationToken);
                return new ApiResult<TodoItemDto>
                {
                    Value = value,
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                };
            }
            return new ApiResult<TodoItemDto>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }

        public async Task<ApiResult<TodoItemDto>> PutTodoItemAsync(Guid id, TodoItemUpdateDto todoItemUpdate, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.PutAsJsonAsync($"{RoutePrefix}/{id}", todoItemUpdate, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TodoItemDto>(cancellationToken);
                return new ApiResult<TodoItemDto>
                {
                    Value = value,
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                };
            }
            return new ApiResult<TodoItemDto>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }

        public async Task<ApiResult<TodoItemDto>> DeleteTodoItemAsync(Guid id, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.DeleteAsync($"{RoutePrefix}/{id}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                // 204 No Content — no body to deserialize.
                return new ApiResult<TodoItemDto>
                {
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                };
            }
            return new ApiResult<TodoItemDto>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }
    }
}
