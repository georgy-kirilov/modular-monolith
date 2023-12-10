using Accounts.Database;
using Accounts.Database.Entities;
using Accounts.Services;
using Shared.Configuration;
using Shared.Database;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;

namespace Accounts;

public static class ModuleRegistration
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabase<AccountsDbContext>(configuration, AccountsDbContext.Schema)
            .AddValidatorsFromAssemblyContaining<AccountsDbContext>()
            .AddTransient<JwtAuthService>()
            .AddTransient<AccountEmailService>();

        var identityOptions = configuration.GetValueOrThrow<IdentityOptions>("Accounts:Identity");

        services
            .AddSingleton(identityOptions)
            .AddIdentityCore<User>(options =>
            {
                options.Password = identityOptions.Password;
                options.User = identityOptions.User;
                options.SignIn = identityOptions.SignIn;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AccountsDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
