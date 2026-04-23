using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.MsSql;
using TodoList.Api.Infrastructure.Persistance;

namespace TodoList.Api.Tests.Infrastructure;

/// <summary>
/// Shared integration-test fixture.
///
/// Lifecycle:
///   - InitializeAsync: starts a SQL Server 2025 container, builds the API factory pointed at it,
///     applies EF Core migrations, and configures Respawn.
///   - ResetDatabaseAsync: truncates all data tables (preserving schema and migration history)
///     so each test starts from a clean slate. Call this from each test's InitializeAsync.
///   - DisposeAsync: tears down the container and the API factory.
///
/// One container is shared across every test class via <see cref="IntegrationCollection"/>,
/// so the ~5–10s container startup cost is paid only once per test run.
/// </summary>
public sealed class IntegrationFixture : IAsyncLifetime
{
    /// <summary>
    /// Connection string pointing at the test database (created on first MigrateAsync).
    /// </summary>
    public string ConnectionString { get; private set; } = null!;
    private const string SaPassword = "T3stP@ssw0rd!";
    private const string DatabaseName = "TodoListTestsDb";

    /// <summary>
    /// Testcontainers-managed SQL Server instance used for the duration of all tests.
    /// </summary>
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2025-latest")
        .WithPassword(SaPassword)
        .Build();

    /// <summary>
    /// Respawner instance used to reset the database to an empty state between tests. Configured in InitializeAsync.
    /// </summary>
    private Respawner _respawner = null!;

    /// <summary>
    /// API factory hosting the TodoList.Api for tests.
    /// </summary>
    public TodoListApiFactory Factory { get; private set; } = null!;

    /// <summary>
    /// Starts the SQL Server container, applies EF Core migrations, and configures Respawn.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        ConnectionString = new SqlConnectionStringBuilder(_dbContainer.GetConnectionString())
        {
            InitialCatalog = DatabaseName,
            TrustServerCertificate = true,
        }.ConnectionString;

        Factory = new TodoListApiFactory(ConnectionString);

        // Apply migrations once at fixture startup so the schema matches production.
        using (var scope = Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TodoListDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        // Configure Respawn to truncate all dbo tables except the EF migrations table.
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new[] { "dbo" },
            TablesToIgnore =
            [
                new("__EFMigrationsHistory"),
            ],
        });
    }

    /// <summary>
    /// Resets all data tables to an empty state. Call from each test's InitializeAsync
    /// to guarantee isolation between tests sharing the same container.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    /// <summary>
    /// Disposes the API factory and stops the SQL Server container. Called once after all tests.
    /// </summary>
    public async Task DisposeAsync()
    {
        Factory.Dispose();
        await _dbContainer.DisposeAsync();
    }
}
