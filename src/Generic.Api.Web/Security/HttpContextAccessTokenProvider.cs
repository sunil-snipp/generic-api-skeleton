using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Generic.Api.Web.Security;

public sealed class HttpContextAccessTokenProvider(IHttpContextAccessor httpContextAccessor) : ICurrentAccessTokenProvider
{
    public string GetCurrentToken()
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization].ToString();
        const string bearerPrefix = "Bearer ";

        if (string.IsNullOrWhiteSpace(authorizationHeader) ||
            !authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new AuthenticationException("A valid Bearer token is required.");
        }

        var token = authorizationHeader[bearerPrefix.Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new AuthenticationException("A valid Bearer token is required.");
        }

        return token;
    }
}
