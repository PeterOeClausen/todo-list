using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TodoList.Api.Client;
using TodoList.Api.Dtos;

namespace TodoList.Api.Mcp.Tools;

[McpServerToolType]
public class TodoItemTools(TodoListApiClient client)
{
    [McpServerTool, Description("Add a new item to a todo list.")]
    public async Task<string> AddTodoItem(
        [Description("The GUID of the todo list to add the item to")] string listId,
        [Description("Title of the new todo item")] string title,
        [Description("Whether the item is already checked (default: false)")] bool @checked = false,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(listId, out var listGuid))
            return "Error: Invalid list ID format. Expected a GUID.";

        var result = await client.TodoItems.PostTodoItemAsync(
            new TodoItemCreateDto { TodoListId = listGuid, Title = title, Checked = @checked },
            cancellationToken);

        if (!result.IsSuccess)
            return $"Error {result.StatusCode}: {result.ErrorMessage}";

        return JsonSerializer.Serialize(result.Value);
    }

    [McpServerTool, Description("Update the title or checked state of a todo item.")]
    public async Task<string> UpdateTodoItem(
        [Description("The GUID of the todo item to update")] string id,
        [Description("New title for the item")] string title,
        [Description("Whether the item is checked")] bool @checked,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var guid))
            return "Error: Invalid item ID format. Expected a GUID.";

        var result = await client.TodoItems.PutTodoItemAsync(
            guid,
            new TodoItemUpdateDto { Title = title, Checked = @checked },
            cancellationToken);

        if (!result.IsSuccess)
            return $"Error {result.StatusCode}: {result.ErrorMessage}";

        return JsonSerializer.Serialize(result.Value);
    }

    [McpServerTool, Description("Delete a todo item by ID.")]
    public async Task<string> DeleteTodoItem(
        [Description("The GUID of the todo item to delete")] string id,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var guid))
            return "Error: Invalid item ID format. Expected a GUID.";

        var result = await client.TodoItems.DeleteTodoItemAsync(guid, cancellationToken);
        if (!result.IsSuccess)
            return $"Error {result.StatusCode}: {result.ErrorMessage}";

        return $"Deleted todo item {id}";
    }
}
