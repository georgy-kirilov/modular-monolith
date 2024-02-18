using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;

namespace Accounts.Features.Login;

public sealed class JwtPrivateKey(RSA value)
{
    public RSA Value { get; } = value;
}

public static class JwtPrivateKeyProviderServiceInstaller
{
    public static IServiceCollection AddJwtPrivateKey(this IServiceCollection services, IConfiguration configuration)
    {
        var privateKeyPassword = configuration.GetValueOrThrow<string>("JWT_PRIVATE_KEY_PASSWORD");

        var jwtPrivateKeyCertificate = new X509Certificate2("/app/accounts/jwt-private-key.pfx", privateKeyPassword);

        ArgumentNullException.ThrowIfNull(jwtPrivateKeyCertificate, nameof(jwtPrivateKeyCertificate));
        
        var rsaPrivateKey = jwtPrivateKeyCertificate.GetRSAPrivateKey();

        ArgumentNullException.ThrowIfNull(rsaPrivateKey, nameof(rsaPrivateKey));

        services.AddSingleton(new JwtPrivateKey(rsaPrivateKey));

        return services;
    }
}
