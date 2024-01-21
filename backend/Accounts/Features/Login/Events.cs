using Microsoft.Extensions.Logging;
using MassTransit;
using Accounts.Services;
using Accounts.Database.Entities;

namespace Accounts.Features.Login;

public static class EmailConfirmationRequired
{
    public sealed record Message(User User);

    public sealed class Consumer(ILogger<Consumer> _logger, AccountEmailService _emailService) : IConsumer<Message>
    {
        public async Task Consume(ConsumeContext<Message> context)
        {
            _logger.LogInformation("Email confirmation for user with ID '{Id}' is required before logging in.",
                context.Message.User.Id);

            await _emailService.SendEmailConfirmation(context.Message.User, context.CancellationToken);
        }
    }
}
