using Microsoft.AspNetCore.Routing;

namespace Shared.Api;

public static class EndpointRegistration
{
    public static IEndpointRouteBuilder MapApiEndpoints<TProgram>(this IEndpointRouteBuilder routeBuilder)
    {
        var endpointTypes = typeof(TProgram).Assembly
            .GetTypes()
            .Where(t =>
                typeof(IEndpoint).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            var instance = Activator.CreateInstance(type) as IEndpoint ?? throw new FailedToRegisterApiEndpointException(type);
            instance.Map(routeBuilder);
        }

        return routeBuilder;
    }
}
