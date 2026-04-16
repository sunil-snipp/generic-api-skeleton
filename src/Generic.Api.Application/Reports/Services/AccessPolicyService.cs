using Generic.Api.Application.Auth.Abstractions;
using Generic.Api.Application.Reports.Abstractions;

namespace Generic.Api.Application.Reports.Services;

public sealed class AccessPolicyService(IExternalIdentityClient externalIdentityClient) : IAccessPolicyService
{
    public async Task<bool> CanAccessReportAsync(string userId, string reportId, CancellationToken cancellationToken = default)
    {
        var reportIds = await GetAccessibleReportIdsAsync(userId, cancellationToken);
        return reportIds.Contains(reportId, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyCollection<string>> GetAccessibleReportIdsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var profile = await externalIdentityClient.GetAccessProfileAsync(userId, cancellationToken);
        return profile.ReportIds;
    }
}
