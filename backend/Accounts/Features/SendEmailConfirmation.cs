using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Shared.Api;
using Shared.Validation;
using Accounts.Database.Entities;
using Accounts.Services;

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
        AccountEmailService accountEmailService,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return new Error("UserNotFound", "No user with such email was found.").ToValidationProblem();
        }

        await accountEmailService.SendEmailConfirmation(user, cancellationToken);

        return Results.Ok();
    }

    public sealed record Request(string Email);
}
