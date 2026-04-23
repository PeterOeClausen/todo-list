# todo-list

This is a small Todo List application I built to showcase how I code. It is a full-stack solution with a .NET Web API backend, a typed C# client library, a React frontend, and an integration-test suite backed by real SQL Server containers.

The scope intentionally stays small — a handful of endpoints around todo lists and todo items — so that the interesting parts are the choices around structure, testing, and developer experience rather than the domain itself.

## What's here

- A REST API built on **.NET 10** with **Entity Framework** against an **SQL Server**, using **Mapster** for DTO mapping, and OpenAPI/Swagger for documentation.
- A strongly-typed **C# client** for the API that can be published as a NuGet package and reused from other services in a microservice setup.
- A React **frontend** (React 19, TypeScript, MUI 9, React Router 6) with light/dark theming, built on Create React App (soon to be updated to Vite).
- An integration-test project that spins up a real SQL Server instance in Docker via **Testcontainers**, runs migrations, and uses **Respawn** to truncate data between tests. Tests call the API through the C# client. This also adds the ability to call an endpoint, and see that the intended database change has happened.
- GitHub CI/CD that builds and tests both frontend and backend. Which triggers only if changes happens in either directory paths.
- nswag script to generate a typescript http client, to easily consume API and stay consistent with dto types.

## Architecture at a glance

```
                     +---------------------------+
                     |  Frontend/todo-app        |
                     |  React + TypeScript + MUI |
                     +------------+--------------+
                                  |  fetch (NSwag-generated TS client)
                                  v
                     +---------------------------+
                     |  TodoList.Api             |
                     |  ASP.NET Core + EF Core   |
                     +------+-------+------------+
                            |       |
         ProjectReference   |       |  UseSqlServer
                            v       v
       +--------------------+     +-----------------------+
       |  TodoList.Api.Dtos |     |  SQL Server (Docker)  |
       +--------------------+     +-----------------------+
                 ^
                 |  ProjectReference
                 |
       +---------+-----------+                +--------------------------+
       | TodoList.Api.Client |<---------------|  TodoList.Api.Tests      |
       |  (typed C# client)  |                |  xUnit + Testcontainers  |
       +---------------------+                |  + Respawn + WebAppFactory|
                                              +--------------------------+
```

The `TodoList.Api.Dtos` project sits in the middle on purpose: DTOs are the contract, the API produces them, and the C# client consumes them without duplicating type definitions.

## Repository layout

| Path                                           | What it is                                                                                                                       |
| ---------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| [`TodoList.Api`](./TodoList.Api)               | The ASP.NET Core Web API — controllers, services, EF Core DbContext, migrations. See [its README](./TodoList.Api/README.md).     |
| [`TodoList.Api.Dtos`](./TodoList.Api.Dtos)     | Request/response DTOs shared between the API and the C# client.                                                                  |
| [`TodoList.Api.Client`](./TodoList.Api.Client) | Typed C# HTTP client over the API, packaged to be shared with other services. See [its README](./TodoList.Api.Client/README.md). |
| [`TodoList.Api.Tests`](./TodoList.Api.Tests)   | Integration tests using a real SQL Server container. See [its README](./TodoList.Api.Tests/README.md).                           |
| [`Frontend/todo-app`](./Frontend/todo-app)     | React + TypeScript SPA. See [its README](./Frontend/todo-app/README.md).                                                         |
| [`Scripts`](./Scripts)                         | Helper scripts, including `generate-ts-client.sh` which regenerates the frontend TS client from the API's OpenAPI spec.          |
| `todo-list.sln`                                | Solution file tying the four .NET projects together.                                                                             |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 18+ and npm (for the frontend)
- [Docker](https://www.docker.com/) — required for local SQL Server and for running the integration tests (which use Testcontainers)

## Quickstart

Run the stack in three terminals. The database is listed first because both the API (on startup) and the integration tests depend on SQL Server.

**1. Start SQL Server in Docker**

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=D3v3l0p3rPassw0rd.' \
  -e 'MSSQL_PID=StandardDeveloper' -p 1433:1433 \
  --name sql2025 --hostname sql2025 \
  -d mcr.microsoft.com/mssql/server:2025-latest
```

**2. Run the API** (applies EF migrations automatically on startup)

```bash
cd TodoList.Api
dotnet run --launch-profile https
```

The API listens on <https://localhost:5000> and Swagger UI is served at <https://localhost:5000/swagger>.

**3. Run the frontend**

```bash
cd Frontend/todo-app
npm install
npm start
```

The app runs at <http://localhost:3000> and is allowlisted by the API's CORS policy out of the box.

## Running the tests

From the repository root:

```bash
dotnet test
```

The test suite boots its own isolated SQL Server container via Testcontainers, so it does not share state with the dev database started above. Full run: ~13.2 seconds on an M4 MacBook Pro.

## Regenerating the frontend TS client

The frontend's API client is NSwag-generated from the OpenAPI spec the API publishes. To refresh it after API changes:

```bash
cd Scripts
./generate-ts-client.sh
```

See [`Scripts/generate-ts-client.sh`](./Scripts/generate-ts-client.sh) for the exact invocation.
