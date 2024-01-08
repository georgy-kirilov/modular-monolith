using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Api;
using Shared.Validation;

namespace Accounts.Features.Register;

public sealed class Endpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("accounts/register", async (
            Request request,
            Handler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(request, cancellationToken);
            return result.Match(res => res.ToOkResult(), err => err.ToValidationProblem());
        })
        .AllowAnonymous()
        .Produces<Response>()
        .ProducesValidationProblem()
        .WithTags("Accounts");
    }
}
