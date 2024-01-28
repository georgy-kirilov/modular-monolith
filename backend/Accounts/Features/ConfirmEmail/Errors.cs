using Shared.Validation;

namespace Accounts.Features.ConfirmEmail;

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
    Error UserIdNotFound { get; }
}

public sealed class EnglishErrors : IErrors
{
    public Error UserIdNotFound => new("User with the given ID was not found.");
}

public sealed class BulgarianErrors : IErrors
{
    public Error UserIdNotFound => new("Потребителят не беше намерен.");
}
