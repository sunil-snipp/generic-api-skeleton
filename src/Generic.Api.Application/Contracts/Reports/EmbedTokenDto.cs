namespace Generic.Api.Application.Contracts.Reports;

public sealed record EmbedTokenDto(
    string ReportId,
    string WorkspaceId,
    string EmbedUrl,
    string Token,
    DateTimeOffset ExpiresAtUtc);
