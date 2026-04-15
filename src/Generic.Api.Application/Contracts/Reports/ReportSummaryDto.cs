namespace Generic.Api.Application.Contracts.Reports;

public sealed record ReportSummaryDto(
    string ReportId,
    string Name,
    string WorkspaceId,
    string DatasetId);
