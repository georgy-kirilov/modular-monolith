using Accounts.Database.Entities;
using Accounts.Services;
using Shared.Api;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using FluentValidation;
using Shared.Authentication;

namespace Accounts.Features;

public static class Login
{
    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder) =>
            builder
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
        JwtSettings jwtSettings,
        JwtAuthService jwtAuthService)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Results.BadRequest(new ValidationFailure[]
            {
                Errors.InvalidLoginCredentials
            });
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Results.BadRequest(new ValidationFailure[]
            {
                Errors.InvalidLoginCredentials
            });
        }

        if (identityOptions.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            return Results.BadRequest(new ValidationFailure[]
            {
                Errors.ConfirmedEmailRequired
            });
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
            ErrorMessage = "You need to confirm your email address."
        };
    }
}
