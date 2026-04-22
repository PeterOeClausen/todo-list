using System.Net.Http.Json;
using TodoList.Api.Dtos;

namespace TodoList.Api.Client
{
    public class TodoListApiClient
    {
        private readonly HttpClient _httpClient;
        public TodoListClient TodoListClient { get; }

        public TodoListApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            TodoListClient = new TodoListClient(_httpClient);
        }
    }
}
