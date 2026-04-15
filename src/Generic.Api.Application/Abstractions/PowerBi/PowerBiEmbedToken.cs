namespace Generic.Api.Application.Abstractions.PowerBi;

public sealed record PowerBiEmbedToken(
    string Token,
    DateTimeOffset ExpiresAtUtc,
    string EmbedUrl);
