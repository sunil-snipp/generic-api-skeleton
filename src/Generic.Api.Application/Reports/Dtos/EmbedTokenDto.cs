namespace Generic.Api.Application.Reports.Dtos;

public sealed record EmbedTokenDto(
    string ReportId,
    string WorkspaceId,
    string EmbedUrl,
    string Token,
    DateTimeOffset ExpiresAtUtc);
