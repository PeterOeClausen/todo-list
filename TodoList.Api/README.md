# TodoList.Api

A .NET 10 ASP.NET Core Web API for managing todo lists and the items inside them. This project is the backend of the solution; the React app under [`Frontend/todo-app`](../Frontend/todo-app) is the primary consumer, and the C# client under [`TodoList.Api.Client`](../TodoList.Api.Client) provides a typed wrapper for .NET consumers.

## Tech stack

- **.NET 10** / ASP.NET Core Web API with attribute-routed controllers.
- **Entity Framework Core 10** with the SQL Server provider. Sensitive data logging is enabled in the DbContext (development convenience — revisit for production).
- **Mapster** for entity ⇄ DTO mapping.
- **OpenAPI + Swagger UI** via `Microsoft.AspNetCore.OpenApi` and `Swashbuckle.AspNetCore.SwaggerUI`. In development the spec is exposed at `/openapi/v1.json` and Swagger UI at `/swagger`.
- **CORS** allows `http://localhost:3000` so the React dev server can call the API directly.

## Project structure

```
TodoList.Api/
├── Configuration/           DatabaseConfiguration — strongly-typed "Database" section of appsettings
├── Controllers/             TodoListController, TodoItemController — thin, delegate to services
├── Infrastructure/
│   └── Persistance/         TodoListDbContext
├── Migrations/              EF Core migrations (applied automatically on startup)
├── Models/
│   └── Entities/            TodoListEntity, TodoItemEntity (incl. IEntityTypeConfiguration)
├── Services/                ITodoListService, ITodoItemService — domain logic, mapping, persistence
├── Properties/              launchSettings.json (https profile on port 5000)
├── Program.cs               Composition root: DI, CORS, DB, middleware, migrate-on-startup
├── appsettings*.json        Configuration (connection string under "Database")
└── database.Dockerfile      Convenience Dockerfile for a pre-configured SQL Server 2025
```

## Domain model

Two entities with a one-to-many relationship: a `TodoList` has zero or more `TodoItem`s.

| Entity | Fields |
| --- | --- |
| `TodoListEntity` | `Id: Guid`, `Name: string` (required), `TodoItems: ICollection<TodoItemEntity>` |
| `TodoItemEntity` | `Id: Guid`, `Title: string` (required), `Checked: bool`, `TodoListId: Guid` (FK) |

The wire representation is defined separately in [`TodoList.Api.Dtos`](../TodoList.Api.Dtos): `TodoListDto`, `TodoListCreateDto`, `TodoListUpdateDto`, `TodoItemDto`, `TodoItemCreateDto`, `TodoItemUpdateDto`.

## Endpoints

All endpoints return `application/json`. Routing uses `[controller]`, so the controller class name is the URL prefix.

| Method | Route | Body | Returns | Notes |
| --- | --- | --- | --- | --- |
| `GET`    | `/TodoList`        | — | `List<TodoListDto>`  | All lists (without nested items). |
| `GET`    | `/TodoList/{id}`   | — | `TodoListDto`        | One list, including its items. 404 if missing. |
| `POST`   | `/TodoList`        | `TodoListCreateDto` | `TodoListDto` | 201 Created with `Location` header. |
| `PUT`    | `/TodoList/{id}`   | `TodoListUpdateDto` | `TodoListDto` | 404 if missing. |
| `DELETE` | `/TodoList/{id}`   | — | 204 No Content | 404 if missing. |
| `POST`   | `/TodoItem`        | `TodoItemCreateDto` | `TodoItemDto` | 404 if the referenced `TodoListId` does not exist. |
| `PUT`    | `/TodoItem/{id}`   | `TodoItemUpdateDto` | `TodoItemDto` | 404 if missing. |
| `DELETE` | `/TodoItem/{id}`   | — | 204 No Content | 404 if missing. |

Full specification: start the API and browse to <https://localhost:5000/swagger>.

## Configuration

The connection string lives under the `Database` section of `appsettings.json` / `appsettings.Development.json`, and is bound to the `DatabaseConfiguration` record in `Program.cs`:

```json
{
  "Database": {
    "ConnectionString": "Server=127.0.0.1,1433;Database=TodoListDb;User Id=sa;Password=D3v3l0p3rPassw0rd.;Encrypt=True;TrustServerCertificate=True"
  }
}
```

The SQL Server connection uses EF Core's built-in retry (`EnableRetryOnFailure`) for transient failures, and `Program.MigrateDatabase` retries migrations up to 5 times at startup — useful when the API is started immediately alongside a fresh SQL Server container.

## How to run Microsoft SQL Server in Docker

The quickest path is the vanilla image:

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=D3v3l0p3rPassw0rd.' \
  -e 'MSSQL_PID=StandardDeveloper' -p 1433:1433 \
  --name sql2025 --hostname sql2025 \
  -d mcr.microsoft.com/mssql/server:2025-latest
```

Alternatively, [`database.Dockerfile`](./database.Dockerfile) bakes the same env vars into an image you can build and run:

```bash
docker build -f database.Dockerfile -t todolist-sql .
docker run -p 1433:1433 --name sql2025 -d todolist-sql
```

## How to build, run, and test

- **Build:** `dotnet build`
- **Run:** `dotnet run --launch-profile https` (serves at <https://localhost:5000>, Swagger at `/swagger`)
- **Test:** `dotnet test` — runs the integration suite in [`TodoList.Api.Tests`](../TodoList.Api.Tests). Docker must be running.

On startup the API automatically applies EF migrations, so there's no manual database setup step for day-to-day development.

## Working with Entity Framework

Install the EF Core CLI once if you don't have it: `dotnet tool install --global dotnet-ef`.

| Task | Command |
| --- | --- |
| Add a migration | `dotnet ef migrations add <MigrationName>` |
| Apply migrations to the database | `dotnet ef database update` |
| List all migrations | `dotnet ef migrations list` |
| Roll back to a previous migration | `dotnet ef database update <MigrationName>` |
| Remove the last migration (if not applied) | `dotnet ef migrations remove` |

## Request flow

```
HTTP → Controller (TodoListController / TodoItemController)
     → Service (ITodoListService / ITodoItemService)        // validates, maps, calls EF
     → TodoListDbContext (EF Core)
     → SQL Server
     ← Entity → DTO (via Mapster)
     ← ActionResult<DTO>
```

Services return `Tuple<bool, Dto?>` for operations that can fail a lookup (update / delete-by-id / add-item-to-missing-list), letting the controller translate the `false` case into a clean `404 NotFound` without throwing.
