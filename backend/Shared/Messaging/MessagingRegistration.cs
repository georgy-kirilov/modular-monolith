using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Shared.Configuration;

namespace Shared.Messaging;

public static class MessagingRegistration
{
    public static IServiceCollection AddMessaging(this IServiceCollection services,
        IConfiguration configuration,
        Action<MessagingOptions> configureMessaging)
    {
        var username = configuration.GetValueOrThrow<string>("RABBITMQ_USER");
        var password = configuration.GetValueOrThrow<string>("RABBITMQ_PASSWORD");

        services.AddMassTransit(busConfig =>
        {
            var messagingOptions = new MessagingOptions(busConfig);

            configureMessaging(messagingOptions);

            busConfig.SetKebabCaseEndpointNameFormatter();

            busConfig.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host: "rabbitmq", virtualHost: "/", configHost =>
                {
                    configHost.Username(username);
                    configHost.Password(password);
                });

                cfg.AutoStart = true;
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
