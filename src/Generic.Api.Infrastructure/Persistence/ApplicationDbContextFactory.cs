using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Generic.Api.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for <c>dotnet ef</c> commands. Uses a local SQLite file.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=generic-api-design.db")
            .Options;

        return new ApplicationDbContext(options);
    }
}
