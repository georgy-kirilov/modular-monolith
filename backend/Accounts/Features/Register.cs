using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MassTransit;
using FluentValidation;
using Shared.Api;
using Shared.Validation;
using Accounts.Database;
using Accounts.Database.Entities;
using Accounts.Services;
using Accounts.Settings;

namespace Accounts.Features;

public static class Register
{
    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder) => builder
            .MapPost("accounts/register", Handle)
            .AllowAnonymous()
            .WithTags("Accounts");
    }

    public static async Task<IResult> Handle(
        Request request,
        Validator validator,
        UserManager<User> userManager,
        IPublishEndpoint publishEndpoint,
        AccountSettings accountSettings,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            return identityResult
                .Errors
                .Select(err => new Error(err.Code, err.Description))
                .ToArray()
                .ToValidationProblem();
        }

        await publishEndpoint.Publish(new UserAccountCreatedMessage(user), cancellationToken);

        return new Response(accountSettings.Email.EmailConfirmationThresholdInSeconds).ToOkResult();
    }

    public sealed record Request
    (
        string Email,
        string Password,
        string ConfirmPassword
    );

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(AccountSettings accountSettings, AccountsDbContext db)
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Email address is required.")
                .EmailAddress()
                .WithMessage("Email address is not in a valid format.")
                .MustAsync((email, cancellationToken) => db.Users.AllAsync(x => x.Email != email, cancellationToken))
                .WithMessage("Email address already exists.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(accountSettings.Password.RequiredLength)
                .WithMessage($"Password must be at least {accountSettings.Password.RequiredLength} characters long.")
                .NotEqual(x => x.Email)
                .WithMessage("Password cannot be the same as the email address.");

            if (accountSettings.Password.RequireDigit)
            {
                RuleFor(x => x.Password)
                    .Must(password => password.Any(char.IsDigit))
                    .WithMessage("Password must contain at least one digit.");
            }

            if (accountSettings.Password.RequireLowercase)
            {
                RuleFor(x => x.Password)
                    .Must(password => password.Any(char.IsLower))
                    .WithMessage("Password must contain at least one lowercase letter.");
            }

            if (accountSettings.Password.RequireUppercase)
            {
                RuleFor(x => x.Password)
                    .Must(password => password.Any(char.IsUpper))
                    .WithMessage("Password must contain at least one uppercase letter.");
            }

            if (accountSettings.Password.RequireNonAlphanumeric)
            {
                RuleFor(x => x.Password)
                    .Must(password => password.Any(x => !char.IsLetterOrDigit(x)))
                    .WithMessage("Password must contain at least one non-alphanumeric symbol.");
            }

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");
        }
    }

    public sealed record Response(int EmailConfirmationThresholdInSeconds);

    public sealed record UserAccountCreatedMessage(User User);

    public sealed class UserAccountCreatedConsumer(
        AccountEmailService emailService,
        ILogger<UserAccountCreatedConsumer> logger)
        : IConsumer<UserAccountCreatedMessage>
    {
        public async Task Consume(ConsumeContext<UserAccountCreatedMessage> context)
        {
            logger.LogInformation("A user account was created with ID '{UserId}'.", context.Message.User.Id);
            await emailService.SendEmailConfirmation(context.Message.User, context.CancellationToken);
        }
    }
}
