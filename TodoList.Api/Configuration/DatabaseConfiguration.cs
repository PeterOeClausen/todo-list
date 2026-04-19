namespace TodoList.Api.Configuration;

/// <summary>
/// Used to map section "Database" in appsettings.*.json files
/// </summary>
public record DatabaseConfiguration
{
    public required string ConnectionString { get; set; }
}
