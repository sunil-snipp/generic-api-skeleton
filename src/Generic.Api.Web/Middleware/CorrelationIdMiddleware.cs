namespace Generic.Api.Web.Middleware;

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
        await next(context);
    }
}
