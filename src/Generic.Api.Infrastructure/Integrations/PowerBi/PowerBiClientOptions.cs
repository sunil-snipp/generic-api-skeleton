namespace Generic.Api.Infrastructure.Integrations.PowerBi;

public sealed class PowerBiClientOptions
{
    public const string SectionName = "PowerBi";

    public string ApiBaseUrl { get; init; } = "https://api.powerbi.com/v1.0/myorg";
}
