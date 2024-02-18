using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Authentication;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services,
        IHostEnvironment environment)
    {
        var jwtSettings = new JwtSettings
        {
            Issuer = GlobalConstants.ApplicationName,
            Audience = GlobalConstants.ApplicationName,
            LifetimeInSeconds = environment.IsDevelopment() ? (12 * 3600) : (1 * 3600)
        };

        services.AddSingleton(jwtSettings);

        var publicKey = new X509Certificate2("/app/shared/authentication/jwt-public-key.crt").GetRSAPublicKey();

        services.AddAuthentication(x =>
        {
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new RsaSecurityKey(publicKey),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseJwtFromInsideCookie(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (!context.Request.Headers.ContainsKey(JwtAuthConstants.Header))
            {
                var cookie = context.Request.Cookies[JwtAuthConstants.Cookie];

                if (cookie is not null)
                {
                    context.Request.Headers.Append(JwtAuthConstants.Header, $"Bearer {cookie}");
                }
            }

            await next(context);
        });

        return app;
    }
}
