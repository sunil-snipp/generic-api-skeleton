namespace Generic.Api.Application.Common;

public interface IRequestContext
{
    string? CorrelationId { get; }
    string? UserId { get; }
}
