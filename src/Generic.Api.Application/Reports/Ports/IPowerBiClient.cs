namespace Generic.Api.Application.Reports.Ports;

public interface IPowerBiClient
{
    Task<IReadOnlyCollection<PowerBiReport>> GetReportsAsync(string workspaceId, CancellationToken cancellationToken = default);

    Task<PowerBiEmbedToken> GenerateEmbedTokenAsync(
        string workspaceId,
        string reportId,
        string userId,
        CancellationToken cancellationToken = default);
}
