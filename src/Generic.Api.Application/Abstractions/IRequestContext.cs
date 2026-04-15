namespace Generic.Api.Application.Abstractions;

public interface IRequestContext
{
    string? CorrelationId { get; }
    string? UserId { get; }
}
