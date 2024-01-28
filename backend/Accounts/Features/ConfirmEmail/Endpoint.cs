using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Api;
using Shared.Validation;

namespace Accounts.Features.ConfirmEmail;

public sealed class Endpoint : IEndpoint
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
    }
}
