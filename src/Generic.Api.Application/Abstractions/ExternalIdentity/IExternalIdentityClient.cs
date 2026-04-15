namespace Generic.Api.Application.Abstractions.ExternalIdentity;

public interface IExternalIdentityClient
{
    Task<ExternalIdentityToken> GetTokenAsync(string username, string password, CancellationToken cancellationToken = default);

    Task<ExternalIdentityProfile?> GetProfileAsync(string accessToken, CancellationToken cancellationToken = default);

    Task<ExternalIdentityProfile?> GetProfileAsync(CancellationToken cancellationToken = default);

    Task<ExternalIdentityAccessProfile> GetAccessProfileAsync(string userId, CancellationToken cancellationToken = default);
}
