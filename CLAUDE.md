# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Backend (.NET 10)

```bash
# Run the API (from repo root or TodoList.Api/)
dotnet run --project TodoList.Api --launch-profile https
# API serves at https://localhost:5000

# Run all tests (requires Docker for SQL Server container)
dotnet test

# Run a single test
dotnet test --filter "FullyQualifiedName~TodoListControllerTests.GetTodoLists_ReturnsAllLists"

# Add an EF migration
dotnet ef migrations add <MigrationName> --project TodoList.Api
```

### Frontend (React/TypeScript/Vite, from `Frontend/todo-app/`)

```bash
npm start          # Dev server at http://localhost:3000
npm run build      # Production build
npm test           # Run tests once
npm run test:watch # Run tests in watch mode

# Run a single test file
npm test -- --run src/pages/TodoListPage.test.tsx
```

### Regenerate the TypeScript API client

Run this whenever the backend API contract changes:

```bash
# Requires API running at https://localhost:5000 and nswag installed globally
./Scripts/generate-ts-client.sh
```

`Frontend/todo-app/src/clients/todo-api-client.ts` is NSwag-generated — never hand-edit it.

## Architecture

This is a full-stack todo app: a React SPA backed by an ASP.NET Core REST API with SQL Server.

### Project layout

| Project | Role |
|---|---|
| `TodoList.Api` | ASP.NET Core Web API |
| `TodoList.Api.Dtos` | Shared DTO types (used by both API and C# client) |
| `TodoList.Api.Client` | Typed C# HTTP client, publishable as a NuGet package |
| `TodoList.Api.Tests` | xUnit integration tests |
| `Frontend/todo-app` | React 19 SPA |

### Request flow

```
React page (useState/useEffect)
  → NSwag-generated TodoApiClient (src/clients/todo-api-client.ts)
  → HTTPS to https://localhost:5000
  → ASP.NET Core Controller (thin — validates input, delegates)
  → Service (ITodoListService / ITodoItemService) — maps DTOs↔Entities via Mapster, runs validation
  → EF Core DbContext → SQL Server
```

### Key design decisions

**Thin controllers, logic in services.** Controllers translate service results to HTTP status codes (e.g., `(false, null)` tuple → 404). Services own validation, entity mapping (Mapster), and EF coordination.

**Shared DTO project.** `TodoList.Api.Dtos` is referenced by both `TodoList.Api` and `TodoList.Api.Client`, making the C# client typesafe against the real API contract without duplication.

**`ApiResult<T>` in the C# client.** The client never throws on HTTP failures; it always returns `ApiResult<T>` with a `StatusCode` and optional `ErrorMessage`. Services return `Tuple<bool, Dto?>` patterns for the same reason — failures are expected outcomes, not exceptions.

**Database auto-migration on startup.** `Program.MigrateDatabase()` applies EF migrations automatically when the API starts, with exponential-backoff retries. No manual migration step is needed in development.

**Integration tests hit a real database.** Tests use Testcontainers to spin up a SQL Server 2025 container per test run. `WebApplicationFactory<Program>` overrides the DbContext connection string. Respawn truncates data between tests. There are no unit test mocks for the data layer — real EF behaviour is the point.

**NSwag-generated TypeScript client.** The frontend uses a generated client that stays in sync with the OpenAPI spec. Regenerate it via `Scripts/generate-ts-client.sh` when API endpoints change.

**Frontend state is local.** Each page manages its own state with `useState`/`useEffect`. The only shared state is the MUI light/dark theme, handled by `ThemeSwitcherContext`. No Redux or Zustand.

**Frontend API base URL** is hard-coded in `src/clients/todo-api-utils.ts` to `https://localhost:5000`. Change it there for non-local targets.

### Prerequisites

- .NET 10 SDK
- Node.js 18+ and npm
- Docker (required for SQL Server in development and all tests)
