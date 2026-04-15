using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace Generic.Api.Web.Errors;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is KeyNotFoundException or UnauthorizedAccessException or ArgumentException)
        {
            logger.LogWarning(exception, "Handled exception");
        }
        else
        {
            logger.LogError(exception, "Unhandled exception");
        }

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        int statusCode;
        object body;

        switch (exception)
        {
            case KeyNotFoundException ex:
                statusCode = StatusCodes.Status404NotFound;
                body = new ProblemDetailsBody(
                    StatusCodes.Status404NotFound,
                    "Not Found",
                    ex.Message,
                    traceId);
                break;
            case UnauthorizedAccessException ex:
                statusCode = StatusCodes.Status403Forbidden;
                body = new ProblemDetailsBody(
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    ex.Message,
                    traceId);
                break;
            case ArgumentException ex:
                statusCode = StatusCodes.Status400BadRequest;
                body = new ProblemDetailsBody(
                    StatusCodes.Status400BadRequest,
                    "Bad Request",
                    ex.Message,
                    traceId);
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                body = new ProblemDetailsBody(
                    StatusCodes.Status500InternalServerError,
                    "Server Error",
                    "An unexpected error occurred.",
                    traceId);
                break;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(body, cancellationToken);
        return true;
    }

    private sealed record ProblemDetailsBody(
        int Status,
        string Title,
        string Detail,
        string TraceId);
}
