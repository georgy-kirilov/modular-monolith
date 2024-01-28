using System.Net;
using Accounts.Database;
using Accounts.Database.Entities;
using Accounts.Settings;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Configuration;
using Shared.Email;

namespace Accounts.Services;

public sealed record SendAccountConfirmationEmail(Guid UserId);

public sealed class SendAccountConfirmationEmailConsumer(
    TimeProvider _timeProvider,
    AccountSettings _accountSettings,
    ILogger<SendAccountConfirmationEmailConsumer> _logger,
    UserManager<User> _userManager,
    IConfiguration _configuration,
    EmailTemplateRenderer _emailTemplateRenderer,
    AccountsDbContext _dbContext,
    IEmailSender _emailSender) : IConsumer<SendAccountConfirmationEmail>
{
    public async Task Consume(ConsumeContext<SendAccountConfirmationEmail> context)
    {
        var user = await _userManager.FindByIdAsync(context.Message.UserId.ToString());

        if (user is null)
        {
            return;
        }

        if (!user.CanSendEmailConfirmation(
            _timeProvider.GetUtcNow(),
            _accountSettings.Email.EmailConfirmationThresholdInSeconds))
        {
            _logger.LogInformation("Postponing email confirmation for user with ID '{Id}' as the interval since the last email is still within the threshold limit.", user.Id);
            return;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);

        var baseAddress = _configuration.GetValueOrThrow<string>("APP_BASE_ADDRESS");
        var url = $"{baseAddress}/accounts/email-confirmation?userId={user.Id}&token={encodedToken}";
        var html = await _emailTemplateRenderer.RenderAsync("ConfirmEmailTemplate", new() { ["ConfirmationUrl"] = url });

        var emailSent = await _emailSender.SendEmailAsync(
            toEmail: user.Email!,
            toName: user.UserName!,
            subject: "Confirm Your Email",
            html);

        if (!emailSent)
        {
            _logger.LogError("Failed to send email confirmation for user with ID '{Id}'.", user.Id);
            return;
        }

        _logger.LogInformation("Email confirmation has been sent to user with ID '{Id}'.", user.Email);

        var updatedRowsCount = await _dbContext.Users
            .Where(x => x.Id == user.Id)
            .ExecuteUpdateAsync(entity => entity
            .SetProperty(y => y.LastEmailConfirmationSentAt, _timeProvider.GetUtcNow()), context.CancellationToken);

        if (updatedRowsCount != 1)
        {
            _logger.LogError($"Failed to update {nameof(User.LastEmailConfirmationSentAt)} for user with ID '{{Id}}'.", user.Id);
            return;
        }
    }
}
