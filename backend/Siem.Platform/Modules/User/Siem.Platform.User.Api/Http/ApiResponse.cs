namespace Siem.Platform.User.Api.Http;

public sealed record ApiError(string Code, string Message);

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public List<ApiError> Errors { get; init; } = new();
    public string? TraceId { get; init; }
}
