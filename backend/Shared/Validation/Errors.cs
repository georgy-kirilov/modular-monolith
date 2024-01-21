using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Validation;

public sealed record Error(string Field, string Code, string Message)
{
    public Error(string code, string message) : this(string.Empty, code, message) { }

    public Error(string message) : this(string.Empty, string.Empty, message) { }

    public Error[] ToErrorsArray() => [this];

    public static implicit operator Error(string message) => new(message);
}

public sealed class Translations
{
    public required Type BaseType { get; init; }

    public required object English { get; init; }

    public required object Bulgarian { get; init; }
}

public interface IErrorsRegistration
{
    Translations Create();
}

public static class ErrorsServiceRegistration
{
    public static IServiceCollection AddErrors<TProgram>(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        var types = typeof(TProgram).Assembly
            .GetTypes()
            .Where(t =>
                typeof(IErrorsRegistration).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract);

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type) as IErrorsRegistration ?? throw new ArgumentException();
            var translations = instance.Create();
            
            services.AddTransient(translations.BaseType, serviceProvider =>
            {
                string? lang = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Request?.Headers?.AcceptLanguage;
    
                return lang switch
                {
                    "bg" => translations.Bulgarian,
                    _ => translations.English
                };
            });
        }

        return services;
    }
}
