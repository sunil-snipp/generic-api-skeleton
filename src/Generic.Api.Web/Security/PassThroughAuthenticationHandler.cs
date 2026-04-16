using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Generic.Api.Web.Security;

public sealed class PassThroughAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var value = authorizationHeader.ToString();
        const string bearerPrefix = "Bearer ";
        if (!value.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header must use Bearer scheme."));
        }

        var token = value[bearerPrefix.Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult(AuthenticateResult.Fail("Bearer token is missing."));
        }

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var authTicket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(authTicket));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AuthenticateResult.Fail($"Invalid JWT format: {ex.Message}"));
        }
    }
}
