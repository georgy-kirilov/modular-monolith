using Shared.Validation;

namespace Accounts.Features.ResendAccountConfirmationEmail;

public sealed class LocalizedErrors : ILocalizedResource
{
    public Localization Create() => new()
    {
        BaseType = typeof(IErrors),
        English = new EnglishErrors(),
        Bulgarian = new BulgarianErrors()
    };
}

public interface IErrors
{
    Error UserEmailNotFound { get; }
}

public sealed class EnglishErrors : IErrors
{
    public Error UserEmailNotFound => new("User with the given email was not found.");
}

public sealed class BulgarianErrors : IErrors
{
    public Error UserEmailNotFound => new("Потребител с дадения имейл не беше намерен.");
}
