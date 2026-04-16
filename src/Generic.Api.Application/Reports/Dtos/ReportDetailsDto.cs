namespace Generic.Api.Application.Reports.Dtos;

public sealed record ReportDetailsDto(
    string ReportId,
    string Name,
    string WorkspaceId,
    string DatasetId,
    string EmbedUrl);
