namespace Generic.Api.Application.Contracts.Reports;

public sealed record ReportDetailsDto(
    string ReportId,
    string Name,
    string WorkspaceId,
    string DatasetId,
    string EmbedUrl);
