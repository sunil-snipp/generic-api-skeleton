namespace Generic.Api.Application.Auth.Models;

public sealed record ExternalIdentityToken(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string? RefreshToken,
    DateTimeOffset? RefreshTokenExpiresOn);
