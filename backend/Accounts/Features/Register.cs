using Accounts.Database.Entities;
using Shared.Api;
using MassTransit;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Accounts.Database;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Features;

public static class Register
{
    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder) =>
            builder
            .MapPost("accounts/register", Handle)
            .AllowAnonymous()
            .WithTags("Accounts");
    }

    public static async Task<IResult> Handle(
        Request request,
        Validator validator,
        UserManager<User> userManager,
        IBus bus,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            var identityErrors = identityResult.Errors.Select(err => new ValidationFailure
            {
                PropertyName = string.Empty,
                ErrorCode = err.Code,
                ErrorMessage = err.Description
            });

            return Results.BadRequest(identityErrors);
        }

        await bus.Publish(new UserAccountCreatedMessage(user), cancellationToken);

        return Results.Ok();
    }

    public sealed record Request
    (
        string Email,
        string Password,
        string ConfirmPassword
    );

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IdentityOptions identityOptions, AccountsDbContext db)
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
                .MinimumLength(identityOptions.Password.RequiredLength)
                .WithMessage($"Password must be at least {identityOptions.Password.RequiredLength} characters long.")
                .NotEqual(x => x.Email)
                .WithMessage("Password cannot be the same as the email address.");

            if (identityOptions.Password.RequireDigit)
            {
                RuleFor(x => x.Password)
                    .Must(password => password.Any(char.IsDigit))
                    .WithMessage("Password must contain at least one digit.");
            }

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");
        }
    }
}
