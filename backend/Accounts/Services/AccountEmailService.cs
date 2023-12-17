using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using HandlebarsDotNet;
using Shared.Email;
using Shared.Configuration;
using Accounts.Database.Entities;
using Accounts.Settings;
using Accounts.Database;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Services;

public sealed class AccountEmailService(
    TimeProvider timeProvider,
    AccountSettings accountSettings,
    ILogger<AccountEmailService> logger,
    UserManager<User> userManager,
    IConfiguration configuration,
    FilePathResolver filePathResolver,
    AccountsDbContext db,
    IEmailSender emailSender)
{
    public async Task SendEmailConfirmation(User user, CancellationToken cancellationToken)
    {
        if (!user.CanSendEmailConfirmation(timeProvider.GetUtcNow(), accountSettings.Email.EmailConfirmationThresholdInSeconds))
        {
            logger.LogInformation("Postponing email confirmation for user with ID '{Id}' as the interval since the last email is still within the threshold limit.", user.Id);
            return;
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);

        var baseAddress = configuration.GetValueOrThrow<string>("APP_BASE_ADDRESS");
        var url = $"{baseAddress}/accounts/email-confirmation?userId={user.Id}&token={encodedToken}";
        var html = await RenderAsync("ConfirmEmailTemplate", new() { ["ConfirmationUrl"] = url });

        var emailSent = await emailSender.SendEmailAsync(
            toEmail: user.Email!,
            toName: user.UserName!,
            subject: "Confirm Your Email",
            html);

        if (!emailSent)
        {
            logger.LogError("Failed to send email confirmation for user with ID '{Id}'.", user.Id);
            return;
        }

        logger.LogInformation("Email confirmation has been sent to user with ID '{Id}'.", user.Email);

        var updatedRowsCount = await db.Users
            .Where(x => x.Id == user.Id)
            .ExecuteUpdateAsync(entity => entity
            .SetProperty(y => y.LastEmailConfirmationSentAt, timeProvider.GetUtcNow()), cancellationToken);

        if (updatedRowsCount != 1)
        {
            logger.LogError($"Failed to update {nameof(User.LastEmailConfirmationSentAt)} for user with ID '{{Id}}'.", user.Id);
            return;
        }
    }

    private async Task<string> RenderAsync(string templateName, Dictionary<string, object> parameters)
    {
        var path = filePathResolver.GetResourceFilePath($"{templateName}.html");
        var templateContent = await File.ReadAllTextAsync(path);
        var compiledTemplate = Handlebars.Compile(templateContent);
        var html = compiledTemplate.Invoke(parameters);
        return html;
    }
}
