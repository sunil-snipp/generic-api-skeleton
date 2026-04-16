namespace Generic.Api.Application.Reports.Abstractions;

public interface IAccessPolicyService
{
    Task<bool> CanAccessReportAsync(string userId, string reportId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetAccessibleReportIdsAsync(string userId, CancellationToken cancellationToken = default);
}
