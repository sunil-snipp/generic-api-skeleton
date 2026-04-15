using Generic.Api.Application.Abstractions;
using Generic.Api.Application.Abstractions.ExternalIdentity;
using Generic.Api.Application.Abstractions.PowerBi;
using Generic.Api.Application.Abstractions.Reports;
using Generic.Api.Application.Configuration;
using Generic.Api.Application.Contracts.Reports;
using Microsoft.Extensions.Options;

namespace Generic.Api.Application.Services;

public sealed class ReportService(
    IRequestContext requestContext,
    IAccessPolicyService accessPolicyService,
    IPowerBiClient powerBiClient,
    IOptions<PowerBiOptions> options) : IReportService
{
    public async Task<IReadOnlyCollection<ReportSummaryDto>> GetReportsAsync(CancellationToken cancellationToken = default)
    {
        var userId = requestContext.UserId ?? throw new UnauthorizedAccessException("User context is missing.");
        var accessibleIds = await accessPolicyService.GetAccessibleReportIdsAsync(userId, cancellationToken);
        var reports = await powerBiClient.GetReportsAsync(options.Value.WorkspaceId, cancellationToken);

        return reports
            .Where(report => accessibleIds.Contains(report.ReportId, StringComparer.OrdinalIgnoreCase))
            .Select(report => new ReportSummaryDto(report.ReportId, report.Name, report.WorkspaceId, report.DatasetId))
            .ToArray();
    }

    public async Task<ReportDetailsDto?> GetReportByIdAsync(string reportId, CancellationToken cancellationToken = default)
    {
        var userId = requestContext.UserId ?? throw new UnauthorizedAccessException("User context is missing.");
        var hasAccess = await accessPolicyService.CanAccessReportAsync(userId, reportId, cancellationToken);
        if (!hasAccess)
        {
            return null;
        }

        var reports = await powerBiClient.GetReportsAsync(options.Value.WorkspaceId, cancellationToken);
        var report = reports.FirstOrDefault(x => x.ReportId.Equals(reportId, StringComparison.OrdinalIgnoreCase));
        if (report is null)
        {
            return null;
        }

        return new ReportDetailsDto(report.ReportId, report.Name, report.WorkspaceId, report.DatasetId, report.EmbedUrl);
    }

    public async Task<EmbedTokenDto> CreateEmbedTokenAsync(string reportId, CancellationToken cancellationToken = default)
    {
        var userId = requestContext.UserId ?? throw new UnauthorizedAccessException("User context is missing.");
        var hasAccess = await accessPolicyService.CanAccessReportAsync(userId, reportId, cancellationToken);
        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("You do not have access to this report.");
        }

        var token = await powerBiClient.GenerateEmbedTokenAsync(options.Value.WorkspaceId, reportId, userId, cancellationToken);
        return new EmbedTokenDto(reportId, options.Value.WorkspaceId, token.EmbedUrl, token.Token, token.ExpiresAtUtc);
    }
}
