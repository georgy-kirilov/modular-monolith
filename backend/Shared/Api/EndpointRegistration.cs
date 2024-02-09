using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Database;

namespace Shared.Api;

public static class EndpointRegistration
{
    public static IServiceCollection AddApiEndpointsFromAssemblyContaining<T>(this IServiceCollection services)
    {
        var endpointTypes = typeof(T).Assembly
            .GetTypes()
            .Where(t =>
                typeof(IEndpoint).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            services.AddScoped(type);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapApiEndpointsForAssemblyContaining<TProgram>(this IEndpointRouteBuilder baseEndpointRouteBuilder,
        WebApplication app)
    {
        var endpointTypes = typeof(TProgram).Assembly
            .GetTypes()
            .Where(t =>
                typeof(IEndpoint).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract);

        using var scope = app.Services.CreateScope();

        foreach (var type in endpointTypes)
        {
            var endpointInsance = scope.ServiceProvider.GetRequiredService(type)
                as IEndpoint
                ?? throw new FailedToRegisterApiEndpointException(type);

            endpointInsance.Map(baseEndpointRouteBuilder);
        }

        return baseEndpointRouteBuilder;
    }
}
