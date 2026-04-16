namespace Generic.Api.Web.Middleware;

using Generic.Api.Application.Logging.Models;
using Serilog.Context;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId;
        if (context.Request.Headers.TryGetValue(HeaderName, out var values))
        {
            correlationId = values.FirstOrDefault() ?? string.Empty;
        }
        else
        {
            correlationId = string.Empty;
        }

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Append(HeaderName, correlationId);

        // Initialize request logging context with correlation data
        var requestContext = RequestLoggingContext.Current;
        requestContext.RequestId = correlationId;
        requestContext.RequestedAPI = $"{context.Request.Method} {context.Request.Path}";
        requestContext.ClientName = ExtractClientName(context) ?? "Unknown";

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("ClientName", requestContext.ClientName))
        {
            try
            {
                await next(context);
                requestContext.StatusCode = context.Response.StatusCode;
            }
            finally
            {
                // Reset AsyncLocal context to prevent memory leaks in long-running processes
                RequestLoggingContext.Reset();
            }
        }
    }

    /// <summary>
    /// Attempts to extract client name from request in order of precedence:
    /// 1. Route values (client_id)
    /// 2. Query parameters (client)
    /// 3. Custom header (X-Client-Name)
    /// </summary>
    private static string? ExtractClientName(HttpContext context)
    {
        // Try extract from route
        if (context.Request.RouteValues.TryGetValue("client_id", out var clientId))
            return clientId?.ToString();

        // Try extract from query string
        if (context.Request.Query.TryGetValue("client", out var client))
            return client.FirstOrDefault();

        // Try extract from custom header
        return context.Request.Headers.TryGetValue("X-Client-Name", out var clientHeader)
            ? clientHeader.FirstOrDefault()
            : null;
    }
}
