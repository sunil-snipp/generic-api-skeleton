namespace Generic.Api.Application.Logging.Abstractions;

/// <summary>
/// Provides structured logging abstraction for the application.
/// Integrates with Serilog for structured, queryable logs.
/// Automatically enriches logs with request context (RequestId, ClientName, etc.).
/// </summary>
public interface IStructuredLogger
{
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Logs an error message with optional exception details.
    /// </summary>
    void LogError(string message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Logs an error message with option to trigger alerts in production.
    /// </summary>
    void LogError(string message, bool sendAlert, params object[] args);

    /// <summary>
    /// Logs a fatal error (application-level failure).
    /// </summary>
    void LogFatal(string message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Fire-and-forget logging of structured events to external systems (Event Hub, etc.).
    /// </summary>
    Task LogEventAsync(string eventType, object data);
}
