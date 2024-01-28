using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Accounts.Database.Entities;
using MassTransit;

namespace Accounts.Database;

public sealed class AccountsDbContext(DbContextOptions<AccountsDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options)
{
    public const string Schema = "accounts";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<Role>().ToTable("role");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claim");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_role");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claim");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_login");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_token");
    }
}
