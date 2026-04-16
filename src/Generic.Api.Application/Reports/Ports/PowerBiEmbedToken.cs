namespace Generic.Api.Application.Reports.Ports;

public sealed record PowerBiEmbedToken(
    string Token,
    DateTimeOffset ExpiresAtUtc,
    string EmbedUrl);
