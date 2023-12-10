using Microsoft.AspNetCore.Routing;

namespace Shared.Api;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder builder);
}
