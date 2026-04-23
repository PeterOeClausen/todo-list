# TodoList.Api.Client

A strongly-typed C# client for the [`TodoList.Api`](../TodoList.Api). It wraps `HttpClient` (easy to setup in dependency injection using `services.AddHttpClient<TodoListApiClient>(options => ...)`), handles JSON (de)serialization, and returns typed DTOs from [`TodoList.Api.Dtos`](../TodoList.Api.Dtos), so callers never deal with raw `HttpResponseMessage`s or hand-rolled route strings.

The project is designed to be published as a NuGet package so other services (e.g. in a microservice setup) can depend on it and call the Todo List API without duplicating DTOs or endpoint knowledge.

## What's inside

| Type                | Responsibility                                                                                                                            |
| ------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| `TodoListApiClient` | Aggregator — wraps one `HttpClient` and exposes `.TodoLists` and `.TodoItems`.                                                            |
| `TodoListClient`    | Calls `/TodoList` endpoints: `GetTodoListsAsync`, `GetTodoListByIdAsync`, `PostTodoListAsync`, `PutTodoListAsync`, `DeleteTodoListAsync`. |
| `TodoItemClient`    | Calls `/TodoItem` endpoints: `PostTodoItemAsync`, `PutTodoItemAsync`, `DeleteTodoItemAsync`.                                              |
| `ApiResult<T>`      | Uniform success/failure wrapper: `Value`, `IsSuccess`, `StatusCode`, `ErrorMessage`. Never throws on non-2xx responses.                   |

Every call takes a `CancellationToken` and returns `Task<ApiResult<T>>`. On success, `Value` is populated (except for 204 No Content responses). On failure, `StatusCode` and `ErrorMessage` are set so the caller can decide how to react without try/catching HTTP plumbing.

## Installation

Add a project reference while working inside this solution:

```xml
<ItemGroup>
  <ProjectReference Include="..\TodoList.Api.Client\TodoList.Api.Client.csproj" />
</ItemGroup>
```

Once published to NuGet, consumers install it with:

```bash
dotnet add package TodoList.Api.Client
```

## Usage

Register a named `HttpClient` with the base address pointing at the API, then new up the client:

```csharp
using TodoList.Api.Client;
using TodoList.Api.Dtos;

// In Program.cs / Startup
services
    .AddHttpClient<TodoListApiClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:5000/");
    });
```

Inject using dependency injection:

```csharp
public class TodoListConsumer
{
    private readonly TodoListApiClient _api;

    public TodoListConsumer(TodoListApiClient api) => _api = api;

    public async Task<List<TodoListDto>> GetAll(CancellationToken ct)
    {
        var result = await _api.TodoLists.GetTodoListsAsync(ct);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                $"Failed to get lists: HTTP {result.StatusCode} — {result.ErrorMessage}");
        }

        return result.Value!;
    }

    public async Task<TodoListDto> CreateList(string name, CancellationToken ct)
    {
        var result = await _api.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = name }, ct);

        return result.IsSuccess
            ? result.Value!
            : throw new InvalidOperationException(result.ErrorMessage);
    }
}
```

The integration tests in [`TodoList.Api.Tests`](../TodoList.Api.Tests) use this exact client to exercise the API — they're a good reference for real usage patterns.

## Design notes

- **No exceptions for non-2xx responses.** HTTP failures are expected outcomes, not exceptional ones. `ApiResult<T>` forces callers to consider them explicitly.
- **Uses `System.Net.Http.Json`.** No Newtonsoft dependency — serialization uses `System.Text.Json`.
- **`TodoList.Api.Dtos` is a separate project** so the client can be published without dragging in the server's EF Core / ASP.NET dependencies.
- **Sub-clients per resource** (`TodoLists`, `TodoItems`) mirror the controller layout on the server and keep call sites readable: `api.TodoLists.PostTodoListAsync(...)` reads naturally.

## Publishing as a NuGet package

The project targets `net10.0` with `Nullable` and `ImplicitUsings` enabled. To produce a package:

```bash
dotnet pack TodoList.Api.Client/TodoList.Api.Client.csproj -c Release
```

Before publishing you'll typically want to add `PackageId`, `Version`, `Authors`, and `Description` properties to the `.csproj`.
