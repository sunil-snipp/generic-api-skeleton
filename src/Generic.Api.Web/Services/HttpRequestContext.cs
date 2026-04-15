using Generic.Api.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Generic.Api.Web.Services;

public sealed class HttpRequestContext(IHttpContextAccessor httpContextAccessor) : IRequestContext
{
    public string? CorrelationId =>
        httpContextAccessor.HttpContext?.Items["CorrelationId"] as string
        ?? httpContextAccessor.HttpContext?.TraceIdentifier;

    public string? UserId =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
}
