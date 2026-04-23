namespace TodoList.Api.Client
{
    public class TodoListApiClient
    {
        private readonly HttpClient _httpClient;
        public TodoListClient TodoLists { get; }
        public TodoItemClient TodoItems { get; }

        public TodoListApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            TodoLists = new TodoListClient(_httpClient);
            TodoItems = new TodoItemClient(_httpClient);
        }
    }
}
