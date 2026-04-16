namespace Generic.Api.Infrastructure.PowerBi;

public sealed class PowerBiOptions
{
    public const string SectionName = "PowerBi";

    public string TenantId { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;

    public string ClientSecret { get; init; } = string.Empty;

    public string WorkspaceId { get; init; } = string.Empty;

    public string ApiBaseUrl { get; init; } = "https://api.powerbi.com/v1.0/myorg";
}
