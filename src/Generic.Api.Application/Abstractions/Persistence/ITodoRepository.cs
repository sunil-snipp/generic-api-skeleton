using Generic.Api.Domain.Entities;

namespace Generic.Api.Application.Abstractions.Persistence;

public interface ITodoRepository
{
    void Add(TodoItem item);
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
