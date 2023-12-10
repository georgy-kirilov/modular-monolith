using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;

namespace Shared.DataProtection;

public static class DataProtectionRegistration
{
    public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {        
        var certificate = new X509Certificate2
        (
            fileName: "/app/certificates/data-protection.pfx",
            password: configuration.GetValueOrThrow<string>("DATA_PROTECTION_CERTIFICATE_PASSWORD"),
            X509KeyStorageFlags.EphemeralKeySet
        );

        services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/root/.aspnet/DataProtection-Keys"))
            .ProtectKeysWithCertificate(certificate);

        return services;
    }
}
