using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;

namespace Shared.Authentication;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var jwtSettings = new JwtSettings
        {
            Key = configuration.GetValueOrThrow<string>("JWT_KEY"),
            Issuer = GlobalConstants.ApplicationName,
            Audience = GlobalConstants.ApplicationName,
            LifetimeInSeconds = environment.IsDevelopment() ? (12 * 3600) : (1 * 3600)
        };

        services.AddSingleton(jwtSettings);

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseJwtFromInsideCookie(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (!context.Request.Headers.ContainsKey(JwtConstants.Header))
            {
                var cookie = context.Request.Cookies[JwtConstants.Cookie];

                if (cookie is not null)
                {
                    context.Request.Headers.Append(JwtConstants.Header, $"Bearer {cookie}");
                }
            }

            await next(context);
        });

        return app;
    }
}
