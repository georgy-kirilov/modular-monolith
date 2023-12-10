using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Shared.Configuration;

namespace Shared.Messaging;

public static class MessagingRegistration
{
    public static IServiceCollection AddMessaging(this IServiceCollection services,
        IConfiguration configuration,
        Assembly[] consumerAssemblies)
    {
        var username = configuration.GetValueOrThrow<string>("RABBITMQ_USER");
        var password = configuration.GetValueOrThrow<string>("RABBITMQ_PASSWORD");

        var consumerTypes = consumerAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsInterface && !t.IsAbstract && typeof(IConsumer).IsAssignableFrom(t));

        services.AddMassTransit(bus =>
        {
            foreach (var consumerType in consumerTypes)
            {
                bus.AddConsumer(consumerType);
            }

            bus.SetKebabCaseEndpointNameFormatter();

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq", "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
