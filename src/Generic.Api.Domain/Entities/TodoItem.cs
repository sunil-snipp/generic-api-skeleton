namespace Generic.Api.Domain.Entities;

public sealed class TodoItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
