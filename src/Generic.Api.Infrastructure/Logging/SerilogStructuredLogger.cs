namespace Generic.Api.Infrastructure.Logging;

using Generic.Api.Application.Logging.Abstractions;
using Generic.Api.Application.Logging.Models;
using Serilog;
using Serilog.Context;

/// <summary>
/// Serilog-based implementation of structured logging.
/// Automatically enriches logs with request context from RequestLoggingContext.
/// Supports fire-and-forget async event logging without blocking request handling.
/// </summary>
public sealed class SerilogStructuredLogger(ILogger logger) : IStructuredLogger
{
    private const int MaxMessageLength = 3000;

    public void LogInformation(string message, params object[] args)
    {
        var enrichedMessage = EnrichMessage(message);
        logger.Information(enrichedMessage, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        var enrichedMessage = EnrichMessage(message);
        logger.Warning(enrichedMessage, args);
    }

    public void LogError(string message, Exception? exception = null, params object[] args)
    {
        var enrichedMessage = EnrichMessage(message);
        if (exception != null)
            logger.Error(exception, enrichedMessage, args);
        else
            logger.Error(enrichedMessage, args);
    }

    public void LogError(string message, bool sendAlert, params object[] args)
    {
        var enrichedMessage = EnrichMessage(message);
        using (LogContext.PushProperty("SendAlert", sendAlert))
        {
            logger.Error(enrichedMessage, args);
        }
    }

    public void LogFatal(string message, Exception? exception = null, params object[] args)
    {
        var enrichedMessage = EnrichMessage(message);
        if (exception != null)
            logger.Fatal(exception, enrichedMessage, args);
        else
            logger.Fatal(enrichedMessage, args);
    }

    /// <summary>
    /// Fire-and-forget async event logging (non-blocking).
    /// Captures current context immediately to prevent context loss in async scenarios.
    /// </summary>
    public async Task LogEventAsync(string eventType, object data)
    {
        // Capture context snapshot to prevent context loss due to async scheduling
        var ctx = RequestLoggingContext.Current;
        var snapshotRequestId = ctx.RequestId;
        var snapshotClientName = ctx.ClientName;

        await Task.Run(() =>
        {
            using (LogContext.PushProperty("RequestId", snapshotRequestId))
            using (LogContext.PushProperty("ClientName", snapshotClientName))
            {
                logger.Information("Event: {EventType} {@EventData}", eventType, data);
            }
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Enriches the message with current request context information.
    /// Format: [RequestId] [ClientName] OriginalMessage
    /// </summary>
    private static string EnrichMessage(string message)
    {
        var ctx = RequestLoggingContext.Current;
        var requestId = string.IsNullOrWhiteSpace(ctx.RequestId) ? "???" : ctx.RequestId[..8];
        var clientName = string.IsNullOrWhiteSpace(ctx.ClientName) ? "???" : ctx.ClientName;
        return $"[{requestId}] [{clientName}] {message}";
    }
}
