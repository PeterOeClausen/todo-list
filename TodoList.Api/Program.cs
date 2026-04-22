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
