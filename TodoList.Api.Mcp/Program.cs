using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoList.Api.Client;
using TodoList.Api.Mcp.Tools;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(opts => opts.LogToStandardErrorThreshold = LogLevel.Trace);

var baseUrl = builder.Configuration["TodoApiBaseUrl"] ?? "https://localhost:5000";

builder.Services.AddSingleton(_ =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    return new TodoListApiClient(httpClient);
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<TodoListTools>()
    .WithTools<TodoItemTools>();

await builder.Build().RunAsync();
