using Microsoft.AspNetCore.Identity;

namespace Accounts.Settings;

public sealed class AccountSettings : IdentityOptions
{
    public const string Section = "Accounts";

    public required EmailSettings Email { get; init; }
}

public sealed class EmailSettings
{
    public required int EmailConfirmationThresholdInSeconds { get; init; }
}
