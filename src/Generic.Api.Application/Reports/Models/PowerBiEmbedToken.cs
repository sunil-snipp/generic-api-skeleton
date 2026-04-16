namespace Generic.Api.Application.Reports.Models;

public sealed record PowerBiEmbedToken(
    string Token,
    DateTimeOffset ExpiresAtUtc,
    string EmbedUrl);
