using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace Shared.Messaging;

public sealed class MessagingOptions(IBusRegistrationConfigurator _bus)
{
    public void AddConsumersFromAssemblyContaining<TContext>() where TContext : DbContext, IOutboxStore
    {
        var consumerTypes = typeof(TContext).Assembly
            .GetTypes()
            .Where(t => typeof(IConsumer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var consumerType in consumerTypes)
        {
            _bus.AddConsumer(consumerType);
        }

        _bus.AddEntityFrameworkOutbox<TContext>(o =>
        {
            o.QueryDelay = TimeSpan.FromSeconds(5);
            o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
            o.UsePostgres();
            o.UseBusOutbox();
        });

        _bus.AddConfigureEndpointsCallback((context, name, cfg) =>
        {
            cfg.UseEntityFrameworkOutbox<TContext>(context);
        });
    }
}
