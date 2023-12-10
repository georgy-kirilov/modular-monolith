using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Configuration;

namespace Shared.Database;

public static class DatabaseRegistration
{
    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services,
        IConfiguration configuration,
        string schema)
        where TContext : DbContext
    {
        var username = configuration.GetValueOrThrow<string>("DB_USER");
        var password = configuration.GetValueOrThrow<string>("DB_PASSWORD");
        var database = configuration.GetValueOrThrow<string>("DB_NAME");
        var connection = $"Host=db;Username={username};Password={password};Database={database};";

        services.AddDbContext<TContext>(dbContextOptions =>
        {
            dbContextOptions.UseNpgsql(connection, npgsqlOptions =>
            {
                npgsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema)
                    .MigrationsAssembly(typeof(TContext).Assembly.FullName);
            })
            .UseSnakeCaseNamingConvention();
        }); 

        return services;
    }

    public static async Task ApplyMigrationsInDevelopment<TContext>(this WebApplication app)
        where TContext : DbContext
    {
        if (app.Environment.IsDevelopment())
        {
            await using var scope = app.Services.CreateAsyncScope();
            await using var db = scope.ServiceProvider.GetRequiredService<TContext>();
            await db.Database.MigrateAsync();
        }
    }
}
