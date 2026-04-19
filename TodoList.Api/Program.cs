using Microsoft.EntityFrameworkCore;
using TodoList.Api.Configuration;
using TodoList.Api.Infrastructure.Persistance;
using TodoList.Api.Services;

namespace TodoList.Api;

public class Program
{
    public static void Main(string[] args)
    {
        new Program().Run(args);
    }

    private void Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services
        // CORS
        const string CorsPolicy = "AllowLocalDev";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
            {
                policy.WithOrigins(
                    "https://localhost:5000"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        // Database
        var databaseConfiguration = builder.Configuration
            .GetRequiredSection("Database")
            .Get<DatabaseConfiguration>();
        builder.Services
            .AddDbContext<TodoListDbContext>(options =>
                options.UseSqlServer(databaseConfiguration.ConnectionString));

        // Controllers and OpenAPI
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Application services
        builder.Services.AddScoped<TodoListService>();

        // Build
        var app = builder.Build();

        // Add middleware
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

        app.Run();
    }
}
