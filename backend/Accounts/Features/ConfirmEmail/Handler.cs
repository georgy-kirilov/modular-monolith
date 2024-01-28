using Microsoft.AspNetCore.Identity;
using OneOf;
using OneOf.Types;
using Shared.Validation;
using Accounts.Database.Entities;
using Error = Shared.Validation.Error;

namespace Accounts.Features.ConfirmEmail;

public sealed class Handler(
    UserManager<User> _userManager,
    IErrors _errors) : IHandler<Request, OneOf<Success, Error[]>>
{
    public async Task<OneOf<Success, Error[]>> Handle(Request request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return new[] { _errors.UserIdNotFound };
        }

        var identityResult = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!identityResult.Succeeded)
        {
            return identityResult.Errors.Select(err => new Error(err.Code, err.Description)).ToArray();
        }

        return new Success();
    }
}
