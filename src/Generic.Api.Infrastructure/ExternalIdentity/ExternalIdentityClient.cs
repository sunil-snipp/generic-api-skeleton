using Generic.Api.Application.Auth.Models;
using Generic.Api.Application.Auth.Ports;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;

namespace Generic.Api.Infrastructure.ExternalIdentity;

public sealed class ExternalIdentityClient(
    HttpClient httpClient,
    IOptions<ExternalIdentityOptions> options) : IExternalIdentityClient
{
    public async Task<ExternalIdentityToken> GetTokenAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Username and password are required.");
        }

        var tokenPath = options.Value.TokenPathTemplate.Replace("{apiVersion}", options.Value.ApiVersion, StringComparison.OrdinalIgnoreCase);
        var payload = new ExternalIdentityTokenRequest(username, password);

        var response = await httpClient.PostAsJsonAsync(tokenPath, payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<ExternalIdentityTokenResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("ExternalIdentity token response was empty.");

        return new ExternalIdentityToken(
            tokenResponse.AccessToken,
            tokenResponse.TokenType,
            tokenResponse.ExpiresIn,
            tokenResponse.RefreshToken,
            tokenResponse.RefreshTokenExpiresOn);
    }

    public async Task<ExternalIdentityProfile?> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(options.Value.Username) || string.IsNullOrWhiteSpace(options.Value.Password))
        {
            throw new InvalidOperationException("ExternalIdentity credentials are missing. Configure ExternalIdentity:Username and ExternalIdentity:Password.");
        }

        var token = await GetTokenAsync(options.Value.Username, options.Value.Password, cancellationToken);
        return await GetProfileAsync(token.AccessToken, cancellationToken);
    }

    public async Task<ExternalIdentityProfile?> GetProfileAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ArgumentException("Access token is required.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var profilePath = options.Value.ProfilePathTemplate.Replace("{apiVersion}", options.Value.ApiVersion, StringComparison.OrdinalIgnoreCase);
        var response = await httpClient.GetAsync(profilePath, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new AuthenticationException("External identity rejected the supplied bearer token.");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException("External identity denied access to the profile.");
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ExternalIdentityProfileResponse>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            return null;
        }

        var associatedClients = payload.AssociatedClients?
            .Select(x => new ExternalIdentityAssociatedClient(x.ClientId, x.ClientName, x.ExternalClientId, x.RoleName))
            .ToArray()
            ?? [];

        return new ExternalIdentityProfile(
            payload.UserId,
            payload.UserName,
            payload.Email,
            payload.FirstName,
            payload.LastName,
            payload.PhoneNumber,
            associatedClients);
    }

    public async Task<ExternalIdentityAccessProfile> GetAccessProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var profile = await GetProfileAsync(cancellationToken);
        if (profile is null)
        {
            return new ExternalIdentityAccessProfile(userId, [], [], []);
        }

        var resolvedUserId = profile.UserId.ToString();
        var groupIds = profile.AssociatedClients
            .Select(client => client.ClientId.ToString())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var campaignIds = profile.AssociatedClients
            .Select(client => client.ExternalClientId)
            .Where(externalClientId => !string.IsNullOrWhiteSpace(externalClientId))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new ExternalIdentityAccessProfile(resolvedUserId, campaignIds, groupIds, []);
    }
}
