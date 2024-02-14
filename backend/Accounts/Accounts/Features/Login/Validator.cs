using FluentValidation;
using Shared.Validation;

namespace Accounts.Features.Login;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator(IErrors errors)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithError(errors.EmailRequired);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithError(errors.PasswordRequired);
    }
}
