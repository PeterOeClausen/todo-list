# TodoList.Api
A .NET 10.0 Web API project for managing a simple to-do list application.

## How to build, run and test
- Build: `dotnet build`
- Run: `dotnet run --launch-profile https`
- Test: `dotnet test`

## How to run Microsoft SQL Server in Docker
How to run the SQL Server as a Docker container: 
```
docker run \
-e "ACCEPT_EULA=Y" \
-e "MSSQL_SA_PASSWORD=D3v3l0p3rPassw0rd!" \
-e "MSSQL_PID=StandardDeveloper" \
-p 1433:1433 \
--name sql2025 \
--hostname sql2025 \
-d \ 
mcr.microsoft.com/mssql/server:2025-latest
```

## Working with Entity Framework
- To add a migration: `dotnet ef migrations add <MigrationName>`
- To apply the migration to the database: `dotnet ef database update`
- To list all migrations: `dotnet ef migrations list`
- To go back to a previous migration: `dotnet ef database update <MigrationName>`
- To remove the last migration (if not applied to the database): `dotnet ef migrations remove`
