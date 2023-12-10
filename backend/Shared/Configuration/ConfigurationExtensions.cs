using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.Configuration;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        services.AddTransient<FilePathResolver>();
        return services;
    }

    public static T GetValueOrThrow<T>(this IConfiguration configuration, string section)
    {
        var sectionData = configuration.GetSection(section);

        if (sectionData is null || !sectionData.Exists())
        {
            throw new FailedToLoadConfigurationValueException(section);
        }

        return sectionData.Get<T>() ?? throw new FailedToLoadConfigurationValueException(section);
    }

    public static IConfigurationBuilder AddAppSettings<T>(this IConfigurationBuilder configuration,
        IHostEnvironment environment)
    {
        var projectName = typeof(T).Assembly.GetName().Name;

        var appsettingsPath = environment.IsDevelopment() ?
            $"/src/{projectName}/appsettings.json" :
            $"./{projectName}/appsettings.json";

        configuration.AddJsonFile(appsettingsPath, optional: false, reloadOnChange: false);

        if (environment.IsDevelopment())
        {
            configuration.AddJsonFile($"/src/{projectName}/appsettings.Development.json",
                optional: false,
                reloadOnChange: false);
        }

        return configuration;
    }
}
