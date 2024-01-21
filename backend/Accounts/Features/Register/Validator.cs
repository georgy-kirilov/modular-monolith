using Accounts.Database;
using Accounts.Settings;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Validation;

namespace Accounts.Features.Register;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator(IErrors _errors, AccountSettings _accountSettings, AccountsDbContext _db)
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithError(_errors.EmailAddressIsRequired)
            .EmailAddress()
            .WithError(_errors.EmailAddressInvalidFormat)
            .MustAsync((email, _) => _db.Users.AllAsync(x => x.Email != email, _))
            .WithError(_errors.EmailAddressAlreadyExists);

        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password)
            .WithError(_errors.PasswordMismatch);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithError(_errors.PasswordIsRequired)
            .MinimumLength(_accountSettings.Password.RequiredLength)
            .WithError(_errors.PasswordTooShort, _accountSettings.Password.RequiredLength)
            .NotEqual(x => x.Email)
            .WithError(_errors.PasswordCannotMatchEmailAddress);

        if (_accountSettings.Password.RequireDigit)
        {
            RuleFor(x => x.Password).Must(password => password.Any(char.IsDigit))
                .WithError(_errors.PasswordRequiresDigit);
        }

        if (_accountSettings.Password.RequireLowercase)
        {
            RuleFor(x => x.Password).Must(password => password.Any(char.IsLower))
                .WithError(_errors.PasswordRequiresLowercase);
        }

        if (_accountSettings.Password.RequireUppercase)
        {
            RuleFor(x => x.Password).Must(password => password.Any(char.IsUpper))
                .WithError(_errors.PasswordRequiresUppercase);
        }

        if (_accountSettings.Password.RequireNonAlphanumeric)
        {
            RuleFor(x => x.Password).Must(password => password.Any(x => !char.IsLetterOrDigit(x)))
                .WithError(_errors.PasswordRequiresNonAlphanumeric);
        }
    }
}
