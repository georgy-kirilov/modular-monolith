namespace Shared.Email;

public interface IEmailSender
{
    Task<bool> SendEmailAsync(
        string toEmail,
        string toName,
        string subject,
        string htmlContent);
}
