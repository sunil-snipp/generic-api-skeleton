namespace Generic.Api.Application.Reports.Dtos;

public sealed record ReportSummaryDto(
    string ReportId,
    string Name,
    string WorkspaceId,
    string DatasetId);
