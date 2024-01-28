using Shared.Api;
using Shared.Authentication;
using Shared.Configuration;
using Shared.Database;
using Shared.DataProtection;
using Shared.Email;
using Shared.Logging;
using Shared.Messaging;
using Accounts;
using Accounts.Database;
using Shared;
using Shared.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLogging(builder.Environment);

builder.Configuration.Sources.Clear();

builder.Configuration
    .AddAppSettings<AccountsDbContext>(builder.Environment);

builder.Configuration
    .AddEnvironmentVariables()
    .Build();

builder.Services
    .AddConfiguration()
    .AddLogging()
    .AddSwagger()
    .AddAuthentication(builder.Configuration, builder.Environment)
    .AddDataProtection(builder.Configuration)
    .AddEmail(builder.Configuration)
    .AddDateTime()
    .AddErrors<AccountsDbContext>()
    .AddHandlers<AccountsDbContext>();

builder.Services.AddMessaging<AccountsDbContext>(builder.Configuration,
[
    typeof(AccountsDbContext).Assembly
]);

builder.Services
    .AddAccountsModule(builder.Configuration);

var application = builder.Build();

await application.ApplyMigrations<AccountsDbContext>();

application
    .UseSwaggerInDevelopment()
    .UseJwtFromInsideCookie()
    .UseAuthentication()
    .UseAuthorization();

var group = application.MapGroup("/").RequireAuthorization();

group.MapApiEndpoints<AccountsDbContext>();
group.MapGet("ping", () => new { Message = "Hello" }).AllowAnonymous();

application.Run();
