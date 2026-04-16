using Generic.Api.Application.Auth.Models;
using Generic.Api.Application.Auth.Abstractions;
using Generic.Api.Application.Logging.Abstractions;
using Generic.Api.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Generic.Api.Web.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IExternalIdentityClient externalIdentityClient,
    ICurrentAccessTokenProvider bearerTokenAccessor,
    IStructuredLogger logger) : ControllerBase
{
    [HttpPost("token")]
    [ProducesResponseType(typeof(ExternalIdentityToken), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExternalIdentityToken>> GetToken([FromBody] TokenRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Token request initiated for username: {Username}", request.Username);

        try
        {
            var token = await externalIdentityClient.GetTokenAsync(request.Username, request.Password, cancellationToken);

            logger.LogInformation("Token successfully issued for username: {Username}", request.Username);
            return Ok(token);
        }
        catch (System.Security.Authentication.AuthenticationException ex)
        {
            logger.LogWarning("Authentication failed for username: {Username}, Reason: {Message}", request.Username, ex.Message);
            return BadRequest(new { message = "Invalid username or password." });
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Bad request for token: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError("Token issuance failed for username: {Username}", ex, request.Username);
            throw;
        }
    }

    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ExternalIdentityProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExternalIdentityProfile>> GetProfile(CancellationToken cancellationToken)
    {
        try
        {
            var token = bearerTokenAccessor.GetCurrentToken();
            logger.LogInformation("Profile request initiated, token length: {TokenLength}", token.Length);

            var profile = await externalIdentityClient.GetProfileAsync(token, cancellationToken);

            if (profile is null)
            {
                logger.LogWarning("Profile not found for bearer token");
                return NotFound();
            }

            logger.LogInformation("Profile retrieved successfully");
            return Ok(profile);
        }
        catch (System.Security.Authentication.AuthenticationException ex)
        {
            logger.LogWarning("Authentication failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError("Profile retrieval failed", ex);
            throw;
        }
    }

    public sealed record TokenRequest(string Username, string Password);
}
