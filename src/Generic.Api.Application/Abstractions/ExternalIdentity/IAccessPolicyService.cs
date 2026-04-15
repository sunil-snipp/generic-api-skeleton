namespace Generic.Api.Application.Abstractions.ExternalIdentity;

public interface IAccessPolicyService
{
    Task<bool> CanAccessReportAsync(string userId, string reportId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetAccessibleReportIdsAsync(string userId, CancellationToken cancellationToken = default);
}
