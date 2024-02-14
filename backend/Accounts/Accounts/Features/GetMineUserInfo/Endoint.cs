using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OneOf.Types;
using Shared.Api;
using Shared.Validation;

namespace Accounts.Features.GetMineUserInfo;

public sealed class Endpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("accounts/me/info", async (Handler handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new None(), cancellationToken);
            return result.Match(res => res.ToOkResult(), err => err.ToValidationProblem());
        })
        .RequireAuthorization()
        .Produces<Response>()
        .ProducesValidationProblem()
        .WithTags(nameof(Accounts));
    }
}
