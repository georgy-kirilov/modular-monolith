using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Shared.Api;
using Shared.Authentication;
using Shared.Validation;
using Accounts.Database.Entities;

namespace Accounts.Features;

public static class GetMineUserInfo
{
    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder) => builder
            .MapGet("accounts/me/info", Handle)
            .RequireAuthorization()
            .WithTags("Accounts");
    }

    public static async Task<IResult> Handle(HttpContext http, UserManager<User> userManager)
    {
        var userId = http.User.GetUserId().ToString();

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return new Error("UserNotFound", "User was not found.").ToValidationProblem();
        }

        return new Response(user.Email, user.UserName).ToOkResult();
    }

    public sealed record Response(string? Email, string? Username);
}
