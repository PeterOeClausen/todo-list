using Microsoft.EntityFrameworkCore;
using TodoList.Api.Configuration;
using TodoList.Api.Infrastructure.Persistance;
using TodoList.Api.Services;

namespace TodoList.Api;

public class Program
{
    private string CorsPolicy { get; } = "AllowLocalDev";

    public static void Main(string[] args)
    {
        new Program().Run(args);
    }

    private void Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AddServices(builder);

        var app = builder.Build();

        MigrateDatabase(app);

        AddMiddleware(app);

        app.Run();
    }

    private void AddServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
            {
                policy.WithOrigins(
                    "http://localhost:3000", // Frontend
                    "https://localhost:5000"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        // Database
        var databaseConfiguration = configuration
            .GetRequiredSection("Database")
            .Get<DatabaseConfiguration>();
        services
            .AddDbContext<TodoListDbContext>(options =>
                options.UseSqlServer(databaseConfiguration.ConnectionString));

        // Controllers and OpenAPI
        services.AddControllers();
        services.AddOpenApi();

        // Application services
        services.AddScoped<ITodoListService, TodoListService>();
        services.AddScoped<ITodoItemService, TodoItemService>();
    }

    /// <summary>
    /// Make sure the database exists and is up-to-date with the latest schema.
    /// </summary>
    private void MigrateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoListDbContext>();
        dbContext.Database.Migrate();
    }

    private void AddMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "TodoList API");
            });
        }

        app.UseHttpsRedirection();
        app.UseCors(CorsPolicy);
        app.UseAuthorization();
        app.MapControllers();
    }
}
