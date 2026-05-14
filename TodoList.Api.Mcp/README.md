# TodoList.Api.Mcp

An **MCP (Model Context Protocol) server** that exposes the TodoList REST API as tools for AI assistants such as Claude and Cursor. It uses the typed [`TodoList.Api.Client`](../TodoList.Api.Client) to communicate with the API and runs over **stdio transport**, making it compatible with any MCP host.

## What it does

The server registers two tool groups:

| Tool class      | Tools                                                                                          |
| --------------- | ---------------------------------------------------------------------------------------------- |
| `TodoListTools` | `list_todo_lists`, `get_todo_list`, `create_todo_list`, `update_todo_list`, `delete_todo_list` |
| `TodoItemTools` | `add_todo_item`, `update_todo_item`, `delete_todo_item`                                        |

An AI assistant connected to this server can create, read, update, and delete todo lists and items without any manual API interaction.

## Prerequisites

- .NET 10 SDK
- The TodoList API running at `https://localhost:5000` (or configured via `appsettings.json`)

## Configuration

The API base URL is read from `appsettings.json`:

```json
{
  "TodoApiBaseUrl": "https://localhost:5000"
}
```

Override it by editing that file or by setting the environment variable `TodoApiBaseUrl`.

## Running standalone

```bash
dotnet run --project TodoList.Api.Mcp
```

The server communicates over stdin/stdout, so it is intended to be launched by an MCP host rather than run interactively.

## Connecting to Claude Code

Add the following to your Claude Code MCP configuration (`.claude/settings.json` or `~/.claude/settings.json`):

```json
{
  "mcpServers": {
    "todolist-mcp": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/TodoList.Api.Mcp", "--no-build"],
      "env": {
        "TodoApiBaseUrl": "https://localhost:5000"
      }
    }
  }
}
```

Replace `/path/to/TodoList.Api.Mcp` with the absolute path to this project directory.

## Project structure

```
TodoList.Api.Mcp/
├── Program.cs            # Host setup: registers HttpClient, MCP server, and tool types
├── appsettings.json      # Default configuration (API base URL)
└── Tools/
    ├── TodoListTools.cs  # MCP tools for todo list CRUD
    └── TodoItemTools.cs  # MCP tools for todo item CRUD
```

## Design notes

- **Uses TodoList.Api.Client internally.** The MCP server talks to the API through `TodoListApiClient`. This keeps it decoupled and means it works against any deployed instance of the API, like in a Test or Prod environment, and not just locally.
- **Errors as strings, not exceptions.** `TodoListApiClient` returns `ApiResult<T>`, so every tool method returns a human-readable error string on failure rather than throwing. The AI assistant receives the error message as the tool result and can act on it.
- **Stdio transport.** The server uses `WithStdioServerTransport()`, which is the standard way MCP hosts (Claude Code, Cursor, etc.) spawn and communicate with local servers.
