using Generic.Api.Application.Abstractions.ExternalIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace Generic.Api.Web.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IExternalIdentityClient externalIdentityClient) : ControllerBase
{
    [HttpPost("token")]
    [ProducesResponseType(typeof(ExternalIdentityToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExternalIdentityToken>> GetToken([FromBody] TokenRequest request, CancellationToken cancellationToken)
    {
        var token = await externalIdentityClient.GetTokenAsync(request.Username, request.Password, cancellationToken);
        return Ok(token);
    }

    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ExternalIdentityProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExternalIdentityProfile>> GetProfile(CancellationToken cancellationToken)
    {
        if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeader))
        {
            return Unauthorized(new { message = "Missing Authorization header." });
        }

        var headerValue = authorizationHeader.ToString();
        const string bearerPrefix = "Bearer ";
        if (!headerValue.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new { message = "Authorization header must use Bearer scheme." });
        }

        var token = headerValue[bearerPrefix.Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new { message = "Bearer token is missing." });
        }

        var profile = await externalIdentityClient.GetProfileAsync(token, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    public sealed record TokenRequest(string Username, string Password);
}
