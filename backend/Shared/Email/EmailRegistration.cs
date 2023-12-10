using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;

namespace Shared.Email;

public static class EmailRegistration
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmailSender, SendGridEmailSender>();

        services.AddSingleton(new EmailSettings
        {
            ApiKey = configuration.GetValueOrThrow<string>("EMAIL_API_KEY"),
            FromEmail = configuration.GetValueOrThrow<string>("EMAIL_FROM_ADDRESS"),
            FromName = GlobalConstants.ApplicationName
        });

        return services;
    }
}
