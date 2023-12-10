using Microsoft.EntityFrameworkCore;

namespace Shared.Database;

public abstract class BaseDbContext(DbContextOptions options, string schema)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(schema);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
