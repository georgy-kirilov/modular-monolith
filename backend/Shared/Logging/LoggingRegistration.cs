using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Shared.Configuration;

namespace Shared.Logging;

public static class LoggingRegistration
{
    public static void UseLogging(this IHostBuilder hostBuilder, IHostEnvironment environment, IConfiguration configuration)
    {
        var seqHost = configuration.GetValueOrThrow<string>("SEQ_HOST");

        if (environment.IsDevelopment())
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
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
