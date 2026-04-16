namespace Generic.Api.Application.Reports.Ports;

public interface IAccessPolicyService
{
    Task<bool> CanAccessReportAsync(string userId, string reportId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetAccessibleReportIdsAsync(string userId, CancellationToken cancellationToken = default);
}
