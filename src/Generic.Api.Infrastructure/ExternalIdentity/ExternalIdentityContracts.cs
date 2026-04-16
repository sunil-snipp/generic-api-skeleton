namespace Generic.Api.Infrastructure.ExternalIdentity;

internal sealed record ExternalIdentityTokenRequest(string Username, string Password);

internal sealed record ExternalIdentityTokenResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string? RefreshToken,
    DateTimeOffset? RefreshTokenExpiresOn);

internal sealed record ExternalIdentityProfileResponse(
    int UserId,
    string UserName,
    string? Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    IReadOnlyCollection<ExternalIdentityAssociatedClientResponse>? AssociatedClients);

internal sealed record ExternalIdentityAssociatedClientResponse(
    int ClientId,
    string ClientName,
    string? ExternalClientId,
    string? RoleName);
