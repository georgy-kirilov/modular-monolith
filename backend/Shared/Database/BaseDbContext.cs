using Microsoft.EntityFrameworkCore;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Shared.Messaging;

namespace Shared.Database;

public abstract class BaseDbContext(DbContextOptions options, string schema)
    : DbContext(options), IOutboxStore
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<OutboxState> OutboxStates => Set<OutboxState>();

    public DbSet<InboxState> InboxStates => Set<InboxState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(schema);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddInboxStateEntity();

        modelBuilder.Entity<OutboxMessage>().ToTable("outbox_message");
        modelBuilder.Entity<OutboxState>().ToTable("outbox_state");
        modelBuilder.Entity<InboxState>().ToTable("inbox_state");
    }
}
