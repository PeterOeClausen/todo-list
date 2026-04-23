namespace TodoList.Api.Client;

public class ApiResult<T>
{
    public T? Value { get; init; }
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public string? ErrorMessage { get; init; }
}
