using Shared.Validation;

namespace Accounts.Features.Register;

public sealed class ErrorsRegistration : ILocalizedResource
{
    public Localization Create() => new()
    {
        BaseType = typeof(IErrors),
        English = new English(),
        Bulgarian = new Bulgarian()
    };
}

public interface IErrors
{
    Error EmailAddressIsRequired { get; }

    Error EmailAddressInvalidFormat { get; }

    Error EmailAddressAlreadyExists { get; }

    Error PasswordIsRequired { get; }

    Error PasswordTooShort { get; }

    Error PasswordCannotMatchEmailAddress { get; }

    Error PasswordRequiresDigit { get; }

    Error PasswordRequiresLowercase { get; }

    Error PasswordRequiresUppercase { get; }

    Error PasswordRequiresNonAlphanumeric { get; }

    Error PasswordMismatch { get; }
}

public sealed class English : IErrors
{
    public Error EmailAddressIsRequired => "Email address is required.";

    public Error EmailAddressInvalidFormat => "Email address is not in a valid format.";

    public Error EmailAddressAlreadyExists => "Email address already exists.";

    public Error PasswordIsRequired => "Password is required.";

    public Error PasswordTooShort => "Password must be at least {0} characters long.";

    public Error PasswordCannotMatchEmailAddress => "Password cannot be the same as the email address.";

    public Error PasswordRequiresDigit => "Password must contain at least one digit.";

    public Error PasswordRequiresLowercase => "Password must contain at least one lowercase letter.";

    public Error PasswordRequiresUppercase => "Password must contain at least one uppercase letter.";

    public Error PasswordRequiresNonAlphanumeric => "Password must contain at least one non-alphanumeric symbol.";

    public Error PasswordMismatch => "Passwords do not match.";
}

public sealed class Bulgarian : IErrors
{
    public Error EmailAddressIsRequired => "Трябва да въведете имейл адрес.";

    public Error EmailAddressInvalidFormat => "Имейл адресът е в невалиден формат.";

    public Error EmailAddressAlreadyExists => "Имейл адресът вече съществува.";

    public Error PasswordIsRequired => "Трябва да въведете парола.";

    public Error PasswordTooShort => "Паролата трябва да бъде поне {0} символа дълга.";

    public Error PasswordCannotMatchEmailAddress => "Паролата не може да бъде същата като имейл адреса.";

    public Error PasswordRequiresDigit => "Паролата трябва да съдържа поне една цифра.";

    public Error PasswordRequiresLowercase => "Паролата трябва да съдържа поне една малка буква.";

    public Error PasswordRequiresUppercase => "Паролата трябва да съдържа поне една главна буква.";

    public Error PasswordRequiresNonAlphanumeric => "Паролата трябва да съдържа поне един специален символ.";

    public Error PasswordMismatch => "Паролите не съвпадат.";
}
