namespace Generic.Api.Application.Reports;

public sealed class ReportOptions
{
    public const string SectionName = "PowerBi";

    public string WorkspaceId { get; init; } = string.Empty;
}
