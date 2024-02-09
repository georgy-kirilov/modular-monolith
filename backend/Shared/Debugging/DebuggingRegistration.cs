using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shared.Configuration;

namespace Shared.Debugging;

public static class DebuggingRegistration
{
    public static WebApplicationBuilder EnableDevelopmentDebugging(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment())
        {
            return builder;
        }

        var debugModeEnabled = builder.Configuration.GetValue<bool>("DEBUG_MODE");

        if (!debugModeEnabled)
        {
            return builder;
        }

        int maxSeconds = builder.Configuration.GetValueOrThrow<int>("DEBUG_MODE_ATTACH_PROCESS_WAITING_PERIOD_SECONDS");

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxSeconds, 0, nameof(maxSeconds));

        int elapsedSeconds = 0;

        Console.WriteLine("Application has been started in debug mode. Waiting for a debugging process to be attached...");

        while (true)
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Debugging process has been attached successfully.");
                return builder;
            }

            if (elapsedSeconds > maxSeconds)
            {
                Console.WriteLine("The waiting period for a debugging process to be attached has expired. Application shutting down...");
                Environment.Exit(0);
            }

            Thread.Sleep(1_000);
            elapsedSeconds++;
        }
    }
}
