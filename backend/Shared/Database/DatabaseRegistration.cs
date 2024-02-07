using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Configuration;

namespace Shared.Database;

public static class DatabaseRegistration
{
    public const string MigrationsHistoryTableName = "__ef_migrations_history";

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
                    .MigrationsHistoryTable(MigrationsHistoryTableName, schema)
                    .MigrationsAssembly(typeof(TContext).Assembly.FullName);
            })
            .UseSnakeCaseNamingConvention();
        }); 

        return services;
    }

    public static async Task ApplyMigrations<TContext>(this WebApplication app)
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
