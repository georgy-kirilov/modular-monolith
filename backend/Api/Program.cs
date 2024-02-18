using Accounts;
using Accounts.Database;
using Shared.Api;
using Shared.Authentication;
using Shared.Configuration;
using Shared.Database;
using Shared.DataProtection;
using Shared.Email;
using Shared.Logging;
using Shared.Messaging;
using Shared.Debugging;

var builder = WebApplication.CreateBuilder(args).EnableDevelopmentDebugging();

builder.Host.UseLogging(builder.Environment, builder.Configuration);

builder.Configuration.Sources.Clear();
builder.Configuration
    .AddAppSettings<AccountsDbContext>(builder.Environment)
    .AddEnvironmentVariables()
    .Build();

builder.Services
    .AddConfiguration()
    .AddLogging()
    .AddSwagger()
    .AddAuthentication(builder.Environment)
    .AddDataProtection(builder.Configuration)
    .AddEmail(builder.Configuration)
    .AddSingleton(TimeProvider.System)
    .AddMessaging(builder.Configuration, x =>
    {
        x.AddConsumersFromAssemblyContaining<AccountsDbContext>();
    });

builder.Services
    .AddAccountsModule(builder.Configuration);

var app = builder.Build();

await app.ApplyMigrations<AccountsDbContext>();

app.UseDevelopmentSwaggerUI();
app.UseJwtFromInsideCookie();
app.UseAuthentication();
app.UseAuthorization();

var apiGroup = app.MapGroup("/").RequireAuthorization();

apiGroup.MapApiEndpointsForAssemblyContaining<AccountsDbContext>(app);

app.Run();
