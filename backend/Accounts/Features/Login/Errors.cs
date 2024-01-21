using Shared.Validation;

namespace Accounts.Features.Login;

public sealed class ErrorsRegistration : IErrorsRegistration
{
    public Translations Create() => new()
    {
        BaseType = typeof(IErrors),
        English = new English(),
        Bulgarian = new Bulgarian()
    };
}

public interface IErrors
{
    Error InvalidLoginCredentials { get; }

    Error EmailConfirmationRequired { get; }

    Error EmailRequired { get; }

    Error PasswordRequired { get; }
}

public sealed class English : IErrors
{
    public Error InvalidLoginCredentials => "Invalid email address or password.";

    public Error EmailConfirmationRequired => "Check your inbox for an email confirmation message.";

    public Error EmailRequired => "You need to enter your email address.";

    public Error PasswordRequired => "You need to enter your password.";
}

public sealed class Bulgarian : IErrors
{
    public Error InvalidLoginCredentials => "Невалиден имейл адрес или парола.";

    public Error EmailConfirmationRequired => "Проверете входящата си поща за имейл с потвърждение.";

    public Error EmailRequired => "Трябва да въведете имейла адреса си.";

    public Error PasswordRequired => "Трябва да въведете паролата си.";
}
