using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using MassTransit;
using FluentValidation;
using FluentValidation.Results;
using Accounts.Database.Entities;
using Accounts.Services;
using Shared.Api;
using Shared.Authentication;
using Shared.Validation;

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
        HttpContext http,
        UserManager<User> userManager,
        IdentityOptions identityOptions,
        IBus bus,
        JwtSettings jwtSettings,
        JwtAuthService jwtAuthService)
    {
        var validationResult = await validator.ValidateAsync(request);

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

        if (identityOptions.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            await bus.Publish(new EmailConfirmationRequiredMessage(user));
            return Errors.ConfirmedEmailRequired.ToValidationProblem();
        }

        var token = jwtAuthService.GenerateJwtToken(user.Id, user.UserName!);

        if (request.StoreJwtInCookie)
        {
            jwtAuthService.AppendJwtAuthCookie(http, token);
        }

        var response = new Response(token, jwtSettings.LifetimeInSeconds);
        return Results.Ok(response);        
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public sealed record Request(string Email, string Password, bool StoreJwtInCookie);

    public sealed record Response(string Token, int LifetimeInSeconds);

    public static class Errors
    {
        public static ValidationFailure InvalidLoginCredentials => new()
        {
            PropertyName = string.Empty,
            ErrorCode = "InvalidLoginCredentials",
            ErrorMessage = "Invalid email address or password."
        };

        public static ValidationFailure ConfirmedEmailRequired => new()
        {
            PropertyName = string.Empty,
            ErrorCode = "ConfirmedEmailRequired",
            ErrorMessage = "You need to confirm your email address. Check your inbox for a verification message."
        };
    }

    public sealed record EmailConfirmationRequiredMessage(User User);

    public sealed class EmailConfirmationRequiredConsumer(
        ILogger<EmailConfirmationRequiredConsumer> logger,
        AccountEmailService emailService)
        : IConsumer<EmailConfirmationRequiredMessage>
    {
        public async Task Consume(ConsumeContext<EmailConfirmationRequiredMessage> context)
        {
            logger.LogInformation("Email confirmation for user with ID '{Id}' is required before logging in.", context.Message.User.Id);
            await emailService.SendEmailConfirmation(context.Message.User);
        }
    }
}
