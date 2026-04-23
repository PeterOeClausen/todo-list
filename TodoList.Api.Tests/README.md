# TodoList.Api.Tests

Integration test suite for [`TodoList.Api`](../TodoList.Api). Every test exercises the API end-to-end: real HTTP pipeline, real EF Core, real SQL Server — just running in-process and against a throwaway container.

## What it uses

- **xUnit** — test runner and fixtures.
- **Microsoft.AspNetCore.Mvc.Testing** — hosts the API in-process via `WebApplicationFactory<Program>`, giving tests a real HTTP pipeline without binding a port.
- **Testcontainers.MsSql** — starts a SQL Server 2025 container for the lifetime of the test run. Lightweight Docker containers made for testing — no shared local database state, no pre-seeded data leaking between runs.
- **Respawn** (by Jimmy Bogard) — after migrations are applied once, Respawn truncates all data between tests (preserving schema and `__EFMigrationsHistory`). This is dramatically faster than dropping and recreating the schema per test.
- **TodoList.Api.Client** — the suite calls the API through the same typed C# client a real consumer would use. Tests double as living documentation of the client.
- Docker Engine must be running for test containers to run.

## How tests are wired together

```
[CollectionDefinition("Integration")]
IntegrationCollection
  └── IntegrationFixture  (shared across the whole collection)
        ├── starts MsSqlContainer
        ├── builds TodoListApiFactory pointed at that container
        ├── applies EF migrations once
        └── configures Respawn

Each test class [Collection("Integration")]
  └── InitializeAsync → fixture.ResetDatabaseAsync()  // clean slate
```

- [`Infrastructure/IntegrationFixture.cs`](./Infrastructure/IntegrationFixture.cs) owns the container, the factory, and Respawn. One fixture per test run, so the ~5–10s container startup cost is paid once.
- [`Infrastructure/TodoListApiFactory.cs`](./Infrastructure/TodoListApiFactory.cs) is a `WebApplicationFactory<Program>` that swaps the production `DbContextOptions` for one pointing at the Testcontainers connection string. It uses `ConfigureTestServices` so the override runs after the app's own registration.
- [`Infrastructure/IntegrationCollection.cs`](./Infrastructure/IntegrationCollection.cs) is the xUnit collection definition. Collection tests run sequentially, which is what we want since they share one database.

## How to run

From the repository root:

```bash
dotnet test
```

Or from this directory:

```bash
cd TodoList.Api.Tests
dotnet test
```

Prerequisites:

- .NET 10 SDK
- Docker Engine running — Testcontainers pulls and starts `mcr.microsoft.com/mssql/server:2025-latest` on first run.

You do **not** need the API's own SQL container running. The test suite starts its own, isolated instance.

## How to add a test

New test classes join the `Integration` collection and depend on the fixture:

```csharp
using TodoList.Api.Client;
using TodoList.Api.Dtos;
using TodoList.Api.Tests.Infrastructure;

[Collection(IntegrationCollection.Name)]
public class MyFeatureTests : IAsyncLifetime
{
    private readonly IntegrationFixture _fixture;
    private readonly TodoListApiClient _api;

    public MyFeatureTests(IntegrationFixture fixture)
    {
        _fixture = fixture;
        _api = new TodoListApiClient(_fixture.Factory.CreateClient());
    }

    // Reset DB before every test — isolates test data between cases.
    public Task InitializeAsync() => _fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateList_ReturnsCreatedList()
    {
        var result = await _api.TodoLists.PostTodoListAsync(
            new TodoListCreateDto { Name = "Groceries" }, CancellationToken.None);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal("Groceries", result.Value!.Name);
    }
}
```

[`TodoListControllerTests`](./TodoListControllerTests.cs) and [`TodoItemControllerTests`](./TodoItemControllerTests.cs) are the reference examples.

## Why integration tests over unit tests

Controllers and services here are thin — most non-trivial behaviour emerges from the combination of ASP.NET model binding, EF Core change tracking, and SQL Server semantics. Mocking those out produces tests that pass while real bugs slip through. Real SQL Server + a real HTTP pipeline catches more actual regressions, and with Respawn + a shared container the suite stays fast enough to run on every save.
