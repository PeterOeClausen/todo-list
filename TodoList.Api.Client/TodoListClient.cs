using System.Net.Http.Json;
using TodoList.Api.Dtos;

namespace TodoList.Api.Client
{
    public class TodoListClient
    {
        private const string RoutePrefix = "TodoList";
        private readonly HttpClient _httpClient;

        public TodoListClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResult<List<TodoListDto>>> GetTodoListsAsync(CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(RoutePrefix, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<List<TodoListDto>>(cancellationToken);
                return new ApiResult<List<TodoListDto>>
                {
                    Value = value,
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                };
            }
            return new ApiResult<List<TodoListDto>>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }

        public async Task<ApiResult<TodoListDto>> GetTodoListByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync($"{RoutePrefix}/{id}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TodoListDto>(cancellationToken);
                return new ApiResult<TodoListDto>
                {
                    Value = value,
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                };
            }
            return new ApiResult<TodoListDto>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }

        public async Task<ApiResult<TodoListDto>> PostTodoListAsync(TodoListCreateDto todoList, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.PostAsJsonAsync(RoutePrefix, todoList, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TodoListDto>(cancellationToken);
                return new ApiResult<TodoListDto>
                {
                    Value = value,
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                };
            }
            return new ApiResult<TodoListDto>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }

        public async Task<ApiResult<TodoListDto>> PutTodoListAsync(Guid id, TodoListUpdateDto todoListUpdate, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.PutAsJsonAsync($"{RoutePrefix}/{id}", todoListUpdate, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TodoListDto>(cancellationToken);
                return new ApiResult<TodoListDto>
                {
                    Value = value,
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                };
            }
            return new ApiResult<TodoListDto>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }

        public async Task<ApiResult<TodoListDto>> DeleteTodoListAsync(Guid id, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.DeleteAsync($"{RoutePrefix}/{id}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                // 204 No Content — no body to deserialize.
                return new ApiResult<TodoListDto>
                {
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                };
            }
            return new ApiResult<TodoListDto>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken),
            };
        }
    }
}
