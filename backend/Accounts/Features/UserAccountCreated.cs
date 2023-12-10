using Accounts.Database.Entities;
using Accounts.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Accounts.Features;

public sealed record UserAccountCreatedMessage(User User);

public sealed class UserAccountCreatedConsumer(
    AccountEmailService accountEmailService,
    ILogger<UserAccountCreatedConsumer> logger)
    : IConsumer<UserAccountCreatedMessage>
{
    public async Task Consume(ConsumeContext<UserAccountCreatedMessage> context)
    {
        logger.LogInformation("A user account for '{Username}' was created with ID '{UserId}'.",
            context.Message.User.UserName,
            context.Message.User.Id);

        await accountEmailService.SendEmailConfirmation(context.Message.User);
    }
}
