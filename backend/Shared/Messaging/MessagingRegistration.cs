using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Shared.Configuration;
using System.Text.RegularExpressions;

namespace Shared.Messaging;

public static class MessagingRegistration
{
    public static IServiceCollection AddMessaging(this IServiceCollection services,
        IConfiguration configuration,
        Assembly[] consumerAssemblies)
    {
        var username = configuration.GetValueOrThrow<string>("RABBITMQ_USER");
        var password = configuration.GetValueOrThrow<string>("RABBITMQ_PASSWORD");

        var consumerTypes = consumerAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsInterface && !t.IsAbstract && typeof(IConsumer).IsAssignableFrom(t));

        services.AddMassTransit(bus =>
        {
            foreach (var consumerType in consumerTypes)
            {
                bus.AddConsumer(consumerType);
            }

            bus.SetEndpointNameFormatter(new CustomEndpointNameFormatter());

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq", "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

public partial class CustomEndpointNameFormatter : DefaultEndpointNameFormatter
{
    public override string SanitizeName(string name)
    {
        return base.SanitizeName(name).Replace("_", "-");
    }

    public override string Consumer<T>()
    {
        var type = typeof(T);
        var declaringType = type.DeclaringType;
        bool isConsumerInsideStaticClass = declaringType is not null && declaringType.IsAbstract && declaringType.IsSealed;

        if (isConsumerInsideStaticClass)
        {
            var name = SanitizeName(declaringType!.Name);
            return ToKebabCase(name);
        }

        return base.Consumer<T>();
    }

    private static string ToKebabCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        text = KebabCaseRegex().Replace(text, "-$1").ToLower();
        return text;
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex KebabCaseRegex();
}
