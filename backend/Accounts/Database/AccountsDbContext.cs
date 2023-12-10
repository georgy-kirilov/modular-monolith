using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Accounts.Database.Entities;

namespace Accounts.Database;

public sealed class AccountsDbContext(DbContextOptions<AccountsDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options)
{
    public const string Schema = "accounts";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schema);

        builder.Entity<User>().ToTable("user");
        builder.Entity<Role>().ToTable("role");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claim");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_role");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claim");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_login");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_token");
    }
}
