namespace Generic.Api.Application.Abstractions.ExternalIdentity;

public sealed record ExternalIdentityAccessProfile(
    string UserId,
    IReadOnlyCollection<string> CampaignIds,
    IReadOnlyCollection<string> GroupIds,
    IReadOnlyCollection<string> ReportIds);
