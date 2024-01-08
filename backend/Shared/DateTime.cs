using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public interface IDateTime
{
    DateTimeOffset UtcNow { get; }
}

public sealed class DateTimeProvider : IDateTime
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

public static class DateTimeRegistration
{
    public static IServiceCollection AddDateTime(this IServiceCollection services) =>
        services.AddSingleton<IDateTime, DateTimeProvider>();
}
