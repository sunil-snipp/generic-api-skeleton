using Generic.Api.Application.Reports.Models;

namespace Generic.Api.Application.Reports.Abstractions;

public interface IPowerBiClient
{
    Task<IReadOnlyCollection<PowerBiReport>> GetReportsAsync(string workspaceId, CancellationToken cancellationToken = default);

    Task<PowerBiEmbedToken> GenerateEmbedTokenAsync(
        string workspaceId,
        string reportId,
        string userId,
        CancellationToken cancellationToken = default);
}
