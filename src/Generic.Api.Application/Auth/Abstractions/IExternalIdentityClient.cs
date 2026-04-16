using Generic.Api.Application.Auth.Models;

namespace Generic.Api.Application.Auth.Abstractions;

public interface IExternalIdentityClient
{
    Task<ExternalIdentityToken> GetTokenAsync(string username, string password, CancellationToken cancellationToken = default);

    Task<ExternalIdentityProfile?> GetProfileAsync(string accessToken, CancellationToken cancellationToken = default);

    Task<ExternalIdentityProfile?> GetProfileAsync(CancellationToken cancellationToken = default);

    Task<ExternalIdentityAccessProfile> GetAccessProfileAsync(string userId, CancellationToken cancellationToken = default);
}
