using Generic.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Generic.Api.Infrastructure.Persistence.Configurations;

public sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("todo_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
    }
}
