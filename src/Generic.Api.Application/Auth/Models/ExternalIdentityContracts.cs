namespace Generic.Api.Application.Auth.Models;

public sealed record ExternalIdentityTokenRequest(string Username, string Password);

public sealed record ExternalIdentityTokenResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string? RefreshToken,
    DateTimeOffset? RefreshTokenExpiresOn);

public sealed record ExternalIdentityProfileResponse(
    int UserId,
    string UserName,
    string? Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    IReadOnlyCollection<ExternalIdentityAssociatedClientResponse>? AssociatedClients);

public sealed record ExternalIdentityAssociatedClientResponse(
    int ClientId,
    string ClientName,
    string? ExternalClientId,
    string? RoleName);
