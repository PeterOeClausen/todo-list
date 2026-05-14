using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TodoList.Api.Client;
using TodoList.Api.Dtos;

namespace TodoList.Api.Mcp.Tools;

[McpServerToolType]
public class TodoListTools(TodoListApiClient client)
{
    [McpServerTool, Description("List all todo lists with their item counts.")]
    public async Task<string> ListTodoLists(CancellationToken cancellationToken)
    {
        var result = await client.TodoLists.GetTodoListsAsync(cancellationToken);
        if (!result.IsSuccess)
            return $"Error {result.StatusCode}: {result.ErrorMessage}";

        return JsonSerializer.Serialize(result.Value);
    }

    [McpServerTool, Description("Get a todo list by ID, including all its items.")]
    public async Task<string> GetTodoList(
        [Description("The GUID of the todo list")] string id,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var guid))
            return "Error: Invalid ID format. Expected a GUID.";

        var result = await client.TodoLists.GetTodoListByIdAsync(guid, cancellationToken);
        if (!result.IsSuccess)
            return $"Error {result.StatusCode}: {result.ErrorMessage}";

        return JsonSerializer.Serialize(result.Value);
    }

    [McpServerTool, Description("Create a new todo list.")]
    public async Task<string> CreateTodoList(
        [Description("Name of the new todo list")] string name,
        CancellationToken cancellationToken)
    {
        var result = await client.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = name },
            cancellationToken);

        if (!result.IsSuccess)
            return $"Error {result.StatusCode}: {result.ErrorMessage}";

        return JsonSerializer.Serialize(result.Value);
    }

    [McpServerTool, Description("Rename a todo list.")]
    public async Task<string> UpdateTodoList(
        [Description("The GUID of the todo list to rename")] string id,
        [Description("The new name for the todo list")] string name,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var guid))
            return "Error: Invalid ID format. Expected a GUID.";

        var result = await client.TodoLists.PutTodoListAsync(
            guid,
            new TodoListUpdateDto { Name = name },
            cancellationToken);

        if (!result.IsSuccess)
            return $"Error {result.StatusCode}: {result.ErrorMessage}";

        return JsonSerializer.Serialize(result.Value);
    }

    [McpServerTool, Description("Delete a todo list by ID.")]
    public async Task<string> DeleteTodoList(
        [Description("The GUID of the todo list to delete")] string id,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var guid))
            return "Error: Invalid ID format. Expected a GUID.";

        var result = await client.TodoLists.DeleteTodoListAsync(guid, cancellationToken);
        if (!result.IsSuccess)
            return $"Error {result.StatusCode}: {result.ErrorMessage}";

        return $"Deleted todo list {id}";
    }
}
