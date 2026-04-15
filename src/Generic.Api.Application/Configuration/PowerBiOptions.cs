namespace Generic.Api.Application.Configuration;

public sealed class PowerBiOptions
{
    public const string SectionName = "PowerBi";

    public string WorkspaceId { get; init; } = string.Empty;
}
