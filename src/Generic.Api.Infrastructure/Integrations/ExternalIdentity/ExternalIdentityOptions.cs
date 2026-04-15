namespace Generic.Api.Infrastructure.Integrations.ExternalIdentity;

public sealed class ExternalIdentityOptions
{
    public const string SectionName = "ExternalIdentity";

    public string BaseUrl { get; init; } = string.Empty;

    public string ApiVersion { get; init; } = "1.0";

    public string TokenPathTemplate { get; init; } = "/api/v{apiVersion}/ClientAuth/token";

    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string ProfilePathTemplate { get; init; } = "/api/v{apiVersion}/ClientAuth/profile";
}
