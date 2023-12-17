using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using MassTransit;
using FluentValidation;
using Shared.Api;
using Shared.Authentication;
using Shared.Validation;
using Accounts.Database.Entities;
using Accounts.Services;
using Accounts.Settings;

namespace Accounts.Features;

public static class Login
{
    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder) => builder
            .MapPost("accounts/login", Handle)
            .AllowAnonymous()
            .WithTags("Accounts")
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        Request request,
        Validator validator,
        UserManager<User> userManager,
        HttpContext httpContext,
        AccountSettings accountSettings,
        IBus bus,
        JwtSettings jwtSettings,
        JwtAuthService jwtAuthService,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
        }

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Errors.InvalidLoginCredentials.ToValidationProblem();
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Errors.InvalidLoginCredentials.ToValidationProblem();
        }

        if (user.RequireEmailConfirmation(accountSettings.SignIn.RequireConfirmedEmail))
        {
            await bus.Publish(new EmailConfirmationRequiredMessage(user), cancellationToken);
            return Errors.EmailConfirmationRequired.ToValidationProblem();
        }

        var token = jwtAuthService.GenerateJwtToken(user.Id, user.UserName!);

        if (request.StoreJwtInCookie)
        {
            jwtAuthService.AppendJwtAuthCookie(httpContext, token);
        }

        var response = new Response(token, jwtSettings.LifetimeInSeconds);
        return Results.Ok(response);
    }

    public sealed record Request
    (
        string Email,
        string Password,
        bool StoreJwtInCookie
    );

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("You need to enter your email address.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("You need to enter your password.");
        }
    }

    public static class Errors
    {
        public static Error InvalidLoginCredentials => new(
            "InvalidLoginCredentials",
            "Invalid email address or password.");

        public static Error EmailConfirmationRequired => new(
            "EmailConfirmationRequired",
            "You need to confirm your email. Check your inbox for a confirmation message.");
    }

    public sealed record Response
    (
        string Token,
        int LifetimeInSeconds
    );

    public sealed record EmailConfirmationRequiredMessage(User User);

    public sealed class EmailConfirmationRequiredConsumer(
        ILogger<EmailConfirmationRequiredConsumer> logger,
        AccountEmailService emailService)
        : IConsumer<EmailConfirmationRequiredMessage>
    {
        public async Task Consume(ConsumeContext<EmailConfirmationRequiredMessage> context)
        {
            logger.LogInformation("Email confirmation for user with ID '{Id}' is required before logging in.", context.Message.User.Id);
            await emailService.SendEmailConfirmation(context.Message.User, context.CancellationToken);
        }
    }
}
