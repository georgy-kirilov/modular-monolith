namespace Shared.Authentication;

public sealed class JwtSettings
{
    public required string Key { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required int LifetimeInSeconds { get; init; }
}
