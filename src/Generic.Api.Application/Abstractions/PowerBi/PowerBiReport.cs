namespace Generic.Api.Application.Abstractions.PowerBi;

public sealed record PowerBiReport(
    string ReportId,
    string Name,
    string WorkspaceId,
    string DatasetId,
    string EmbedUrl);
