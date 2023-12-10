namespace Shared.Email;

public sealed class EmailSettings
{
    public required string ApiKey { get; init; }

    public required string FromEmail { get; init; }

    public required string FromName { get; init; }
}
