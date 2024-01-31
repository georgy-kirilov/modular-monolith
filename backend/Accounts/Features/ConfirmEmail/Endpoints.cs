using Accounts.Database;
using Accounts.Database.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Shared.Api;
using Shared.Validation;

namespace Accounts.Features.ConfirmEmail;

public sealed class Endpoints(IHostEnvironment _hostEnvironment) : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("accounts/confirm-email", async (Request request, Handler handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(request, cancellationToken);
            return result.Match(_ => Results.Ok(), err => err.ToValidationProblem());
        })
        .AllowAnonymous()
        .Produces(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .WithTags(nameof(Accounts));

        if (_hostEnvironment.IsDevelopment())
        {
            MapEmailConfirmationDevelopmentLink(builder);
        }
    }

    private static void MapEmailConfirmationDevelopmentLink(IEndpointRouteBuilder builder)
    {
        builder.MapGet("accounts/confirm-email-development-link", async (
            string email,
            AccountsDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is null)
            {
                return Results.NotFound("User with such email was not found.");
            }

            if (user.EmailConfirmed)
            {
                return Results.Problem("User email has already been confirmed.");
            }

            user.EmailConfirmed = true;

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok();
        })
        .AllowAnonymous()
        .WithTags(nameof(Accounts));
    }
}
