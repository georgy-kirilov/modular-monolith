using Accounts.Database.Entities;
using Accounts.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Accounts.Features.Register;

public static class UserAccountCreated
{
    public sealed record Message(User User);

    public sealed class Consumer(ILogger<Consumer> _logger, AccountEmailService _emailService) : IConsumer<Message>
    {
        public async Task Consume(ConsumeContext<Message> context)
        {
            _logger.LogInformation("User account was created with ID '{UserId}'.", context.Message.User.Id);

            await _emailService.SendEmailConfirmation(context.Message.User, context.CancellationToken);
        }
    }
}
