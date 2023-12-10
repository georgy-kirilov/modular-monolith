using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Shared.Logging;

public static class LoggingRegistration
{
    public static void UseLogging(this IHostBuilder hostBuilder, IHostEnvironment environment)
    {
        const string seqHost = "http://seq";

        if (environment.IsDevelopment())
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(seqHost)
                .CreateLogger();
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .MinimumLevel.Warning()
                .WriteTo.Seq(seqHost)
                .CreateLogger();
        }

        hostBuilder.UseSerilog();
    }
}
