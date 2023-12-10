using Accounts.Database.Entities;
using Accounts.Services;
using Shared.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using FluentValidation.Results;

namespace Accounts.Features;

public static class SendEmailConfirmation
{
    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder) =>
            builder
            .MapPost("accounts/send-email-confirmation", Handle)
            .AllowAnonymous()
            .WithTags("Accounts");
    }

    public static async Task<IResult> Handle(
        Request request,
        UserManager<User> userManager,
        AccountEmailService accountEmailService)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Results.BadRequest(new ValidationFailure[]
            {
                new()
                {
                    ErrorCode = "UserNotFound",
                    ErrorMessage = "No user with such email was found."
                }
            });
        }

        await accountEmailService.SendEmailConfirmation(user);

        return Results.Ok();
    }

    public sealed record Request(string Email);
}
