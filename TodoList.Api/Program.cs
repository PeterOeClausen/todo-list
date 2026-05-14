using Microsoft.EntityFrameworkCore;
using TodoList.Api.Commands.TodoItems;
using TodoList.Api.Commands.TodoLists;
using TodoList.Api.Configuration;
using TodoList.Api.Infrastructure.Persistance;

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
                    "http://localhost:3000" // Frontend
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
                options.UseSqlServer(
                    databaseConfiguration!.ConnectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null)));

        // Controllers and OpenAPI
        services.AddControllers();
        services.AddOpenApi();

        // Commands
        services.AddScoped<CreateTodoItem>();
        services.AddScoped<CreateTodoList>();
        services.AddScoped<DeleteTodoItem>();
        services.AddScoped<DeleteTodoList>();
        services.AddScoped<GetTodoListById>();
        services.AddScoped<GetTodoLists>();
        services.AddScoped<UpdateTodoItem>();
        services.AddScoped<UpdateTodoList>();
    }

    /// <summary>
    /// Make sure the database exists and is up-to-date with the latest schema.
    /// </summary>
    private void MigrateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoListDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        const int maxAttempts = 5;
        var delay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                dbContext.Database.Migrate();
                logger.LogInformation("Database migrated successfully on attempt {Attempt}.", attempt);
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(
                    "Database not ready yet (attempt {Attempt}/{MaxAttempts}): {Message}. Retrying in {Delay}s...",
                    attempt, maxAttempts, ex.Message, delay.TotalSeconds);
                Thread.Sleep(delay);
            }
        }

        // Final attempt — let exceptions bubble so the app fails loudly
        // instead of starting in a broken state.
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
