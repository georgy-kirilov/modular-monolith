using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using HandlebarsDotNet;
using Shared.Email;
using Accounts.Database.Entities;
using Shared.Configuration;

namespace Accounts.Services;

public sealed class AccountEmailService(
    FilePathResolver filePathResolver,
    UserManager<User> userManager,
    IEmailSender emailSender,
    ILogger<AccountEmailService> logger)
{
    public async Task SendEmailConfirmation(User user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);

        var url = $"http://localhost:8080/accounts/email-confirmation?userId={user.Id}&token={encodedToken}";
        var html = await RenderAsync("ConfirmEmailTemplate", new() { ["ConfirmationUrl"] = url });

        var emailSent = await emailSender.SendEmailAsync(
            toEmail: user.Email!,
            toName: user.UserName!,
            subject: "Confirm Your Email",
            html);

        if (emailSent)
        {
            logger.LogInformation("An email confirmation has been sent to '{Email}'", user.Email);
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
