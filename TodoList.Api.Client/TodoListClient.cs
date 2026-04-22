using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
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

        public Task<List<TodoListDto>?> GetTodoListsAsync(CancellationToken cancellationToken)
        {
            // Add error handling
            return _httpClient.GetFromJsonAsync<List<TodoListDto>>(RoutePrefix, cancellationToken);
        }

        public Task<TodoListDto?> GetTodoListByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _httpClient.GetFromJsonAsync<TodoListDto>($"{RoutePrefix}/{id}", cancellationToken);
        }

        public async Task<TodoListDto?> PostTodoListAsync(TodoListCreateDto todoList, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync<TodoListCreateDto>(RoutePrefix, todoList, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TodoListDto?>(cancellationToken);
            }

            return null;
        }

        public async Task<TodoListDto?> PutTodoListAsync(Guid id, TodoListUpdateDto todoListUpdate, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync<TodoListUpdateDto>($"{RoutePrefix}/{id}", todoListUpdate, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TodoListDto?>(cancellationToken);
            }
            return null;
        }

        public async Task<bool> DeleteTodoListAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{RoutePrefix}/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
