using Generic.Api.Application.Reports.Abstractions;
using Generic.Api.Application.Reports.Models;

namespace Generic.Api.Infrastructure.PowerBi;

public sealed class PowerBiClient : IPowerBiClient
{
    public Task<IReadOnlyCollection<PowerBiReport>> GetReportsAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PowerBiReport> reports =
        [
            new("sales-overview", "Sales Overview", workspaceId, "dataset-sales", $"https://app.powerbi.com/reportEmbed?reportId=sales-overview&groupId={workspaceId}"),
            new("campaign-performance", "Campaign Performance", workspaceId, "dataset-campaigns", $"https://app.powerbi.com/reportEmbed?reportId=campaign-performance&groupId={workspaceId}")
        ];

        return Task.FromResult(reports);
    }

    public Task<PowerBiEmbedToken> GenerateEmbedTokenAsync(
        string workspaceId,
        string reportId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var embedUrl = $"https://app.powerbi.com/reportEmbed?reportId={reportId}&groupId={workspaceId}";
        return Task.FromResult(new PowerBiEmbedToken(token, DateTimeOffset.UtcNow.AddMinutes(60), embedUrl));
    }
}
