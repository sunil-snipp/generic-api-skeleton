namespace Generic.Api.Application.Auth.Models;

public sealed record ExternalIdentityAccessProfile(
    string UserId,
    IReadOnlyCollection<string> CampaignIds,
    IReadOnlyCollection<string> GroupIds,
    IReadOnlyCollection<string> ReportIds);
