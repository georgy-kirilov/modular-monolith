using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Accounts.Database.Entities;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Shared.Messaging;

namespace Accounts.Database;

public sealed class AccountsDbContext(DbContextOptions<AccountsDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options), IOutboxStore
{
    public const string Schema = "accounts";

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<OutboxState> OutboxStates => Set<OutboxState>();

    public DbSet<InboxState> InboxStates => Set<InboxState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.Entity<InboxState>().ToTable("inbox_state");
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.Entity<OutboxMessage>().ToTable("outbox_message");
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.Entity<OutboxState>().ToTable("outbox_state");

        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<Role>().ToTable("role");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claim");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_role");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claim");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_login");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_token");
    }
}
