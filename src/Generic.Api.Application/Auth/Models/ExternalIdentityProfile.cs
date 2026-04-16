namespace Generic.Api.Application.Auth.Models;

public sealed record ExternalIdentityProfile(
    int UserId,
    string UserName,
    string? Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    IReadOnlyCollection<ExternalIdentityAssociatedClient> AssociatedClients);

public sealed record ExternalIdentityAssociatedClient(
    int ClientId,
    string ClientName,
    string? ExternalClientId,
    string? RoleName);
