namespace Generic.Api.Application.Logging.Models;

/// <summary>
/// Captures per-request logging context using AsyncLocal for thread-safe, scope-aware storage.
/// Automatically available across async call chains without passing parameters.
/// </summary>
public sealed class RequestLoggingContext
{
    private static readonly AsyncLocal<RequestLoggingContext> _current = new();

    public string RequestId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string RequestedAPI { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public DateTime RequestStartTime { get; set; } = DateTime.UtcNow;
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets or initializes the current request context for the async scope.
    /// </summary>
    public static RequestLoggingContext Current
    {
        get => _current.Value ??= new();
        set => _current.Value = value;
    }

    /// <summary>
    /// Clears the context to prevent memory leaks in long-running processes.
    /// Call at the end of request processing (typically in middleware).
    /// </summary>
    public static void Reset() => _current.Value = null;
}
