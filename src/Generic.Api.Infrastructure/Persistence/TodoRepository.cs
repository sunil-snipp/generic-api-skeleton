using Generic.Api.Application.Abstractions.Persistence;
using Generic.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Generic.Api.Infrastructure.Persistence;

public sealed class TodoRepository(ApplicationDbContext db) : ITodoRepository
{
    public void Add(TodoItem item) => db.TodoItems.Add(item);

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.TodoItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
