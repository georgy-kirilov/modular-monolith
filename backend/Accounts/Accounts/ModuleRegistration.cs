using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Shared.Configuration;
using Shared.Database;
using Shared.Validation;
using Shared.Api;
using Accounts.Database;
using Accounts.Database.Entities;
using Accounts.Services;
using Accounts.Settings;

namespace Accounts;

public static class ModuleRegistration
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabase<AccountsDbContext>(configuration, AccountsDbContext.Schema)
            .AddErrorsFromAssemblyContaining<AccountsDbContext>()
            .AddApiEndpointsFromAssemblyContaining<AccountsDbContext>()
            .AddHandlersFromAssemblyContaining<AccountsDbContext>()
            .AddValidatorsFromAssemblyContaining<AccountsDbContext>()
            .AddScoped<EmailTemplateRenderer>();

        var accountSettings = configuration.GetValueOrThrow<AccountSettings>(AccountSettings.Section);

        services
            .AddSingleton(accountSettings)
            .AddIdentityCore<User>(options =>
            {
                options.Password = accountSettings.Password;
                options.User = accountSettings.User;
                options.SignIn = accountSettings.SignIn;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AccountsDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
