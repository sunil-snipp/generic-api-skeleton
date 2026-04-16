using Generic.Api.Application.Auth.Models;
using Generic.Api.Application.Auth.Ports;
using Generic.Api.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Generic.Api.Web.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IExternalIdentityClient externalIdentityClient,
    ICurrentAccessTokenProvider bearerTokenAccessor) : ControllerBase
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
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExternalIdentityProfile>> GetProfile(CancellationToken cancellationToken)
    {
        var token = bearerTokenAccessor.GetCurrentToken();
        var profile = await externalIdentityClient.GetProfileAsync(token, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    public sealed record TokenRequest(string Username, string Password);
}
