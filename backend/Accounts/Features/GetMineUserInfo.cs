using Accounts.Database.Entities;
using Shared.Api;
using Shared.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

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

        var user = await userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException();

        return Results.Ok(new Response
        (
            user.Email,
            user.UserName
        ));
    }

    public sealed record Response(string? Email, string? Username);
}
