namespace Generic.Api.Application.Abstractions.PowerBi;

public interface IPowerBiClient
{
    Task<IReadOnlyCollection<PowerBiReport>> GetReportsAsync(string workspaceId, CancellationToken cancellationToken = default);

    Task<PowerBiEmbedToken> GenerateEmbedTokenAsync(
        string workspaceId,
        string reportId,
        string userId,
        CancellationToken cancellationToken = default);
}
