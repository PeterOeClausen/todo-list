using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TodoList.Api.Infrastructure.Persistance;

namespace TodoList.Api.Tests.Infrastructure;

/// <summary>
/// WebApplicationFactory that hosts the TodoList.Api in-process for integration testing,
/// pointing the EF Core DbContext at a Testcontainers-managed SQL Server instance.
///
/// We use <c>ConfigureTestServices</c> — which runs *after* the user's service
/// registration.
/// </summary>
public sealed class TodoListApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public TodoListApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Replace the real DbContext with a test one pointed at our Testcontainers SQL Server instance.
            services.RemoveAll<DbContextOptions<TodoListDbContext>>();
            services.RemoveAll<TodoListDbContext>();

            services.AddDbContext<TodoListDbContext>(options =>
                options.UseSqlServer(_connectionString));
        });
    }
}
