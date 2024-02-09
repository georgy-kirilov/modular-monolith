using Microsoft.Extensions.DependencyInjection;

namespace Shared.Validation;

public static class Extensions
{
    public static IServiceCollection AddHandlersFromAssemblyContaining<TProgram>(this IServiceCollection services)
    {
        var handlerOpenGenericType = typeof(IHandler<,>);

        var types = typeof(TProgram).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenGenericType));

        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenGenericType);

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(type);
            }
        }

        return services;
    }
}

public interface IHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
