using Accounts.Database.Entities;
using Shared.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Shared.Validation;

namespace Accounts.Features;

public static class ConfirmEmail
{
    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder) => builder
            .MapPost("accounts/confirm-email", Handle)
            .AllowAnonymous()
            .WithTags("Accounts");
    }

    public static async Task<IResult> Handle(Request request, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return new Error("UserNotFound", "User was not found.").ToValidationProblem();
        }

        var identityResult = await userManager.ConfirmEmailAsync(user, request.Token);

        if (!identityResult.Succeeded)
        {
            return identityResult
                .Errors
                .Select(err => new Error(err.Code, err.Description))
                .ToArray()
                .ToValidationProblem();
        }

        return Results.Ok();
    }

    public sealed record Request(Guid UserId, string Token);
}
