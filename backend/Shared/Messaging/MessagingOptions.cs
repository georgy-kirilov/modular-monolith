using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace Shared.Messaging;

public sealed class MessagingOptions(IBusRegistrationConfigurator _bus)
{
    public void AddConsumersFromAssemblyContaining<TContext>() where TContext : DbContext, IOutboxStore
    {
        _bus.AddConsumersFromNamespaceContaining<TContext>();

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
