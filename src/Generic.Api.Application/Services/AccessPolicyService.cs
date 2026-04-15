using Generic.Api.Application.Abstractions.ExternalIdentity;

namespace Generic.Api.Application.Services;

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
