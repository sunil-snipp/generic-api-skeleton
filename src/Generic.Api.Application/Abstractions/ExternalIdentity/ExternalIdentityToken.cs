namespace Generic.Api.Application.Abstractions.ExternalIdentity;

public sealed record ExternalIdentityToken(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string? RefreshToken,
    DateTimeOffset? RefreshTokenExpiresOn);
