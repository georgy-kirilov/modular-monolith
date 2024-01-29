using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Api;
using Shared.Validation;

namespace Accounts.Features.ResendAccountConfirmationEmail;

public sealed class Endpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("accounts/send-email-confirmation", async (
            Request request,
            Handler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(request, cancellationToken);
            return result.Match(_ => Results.Ok(), err => err.ToValidationProblem());
        })
        .AllowAnonymous()
        .Produces(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .WithTags(nameof(Accounts));
    }
}