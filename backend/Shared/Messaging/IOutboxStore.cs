using Microsoft.EntityFrameworkCore;
using MassTransit.EntityFrameworkCoreIntegration;

namespace Shared.Messaging;

public interface IOutboxStore
{
    DbSet<OutboxMessage> OutboxMessages { get; }

    DbSet<OutboxState> OutboxStates { get; }

    DbSet<InboxState> InboxStates { get; }
}
