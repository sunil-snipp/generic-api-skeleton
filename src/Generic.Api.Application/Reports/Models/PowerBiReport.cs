namespace Generic.Api.Application.Reports.Models;

public sealed record PowerBiReport(
    string ReportId,
    string Name,
    string WorkspaceId,
    string DatasetId,
    string EmbedUrl);
