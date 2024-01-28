using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Shared.Configuration;

namespace Shared.Messaging;

public static class MessagingRegistration
{
    public static IServiceCollection AddMessaging<TContext>(this IServiceCollection services,
        IConfiguration configuration,
        Assembly[] consumerAssemblies)
        where TContext : DbContext
    {
        var username = configuration.GetValueOrThrow<string>("RABBITMQ_USER");
        var password = configuration.GetValueOrThrow<string>("RABBITMQ_PASSWORD");

        var consumerTypes = consumerAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsInterface && !t.IsAbstract && typeof(IConsumer).IsAssignableFrom(t));

        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            bus.AddEntityFrameworkOutbox<TContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(5);
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                o.UsePostgres();
                o.UseBusOutbox();
            });

            foreach (var consumerType in consumerTypes)
            {
                bus.AddConsumer(consumerType);
            }

            bus.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseEntityFrameworkOutbox<TContext>(context);
            });

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq", "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.AutoStart = true;
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
