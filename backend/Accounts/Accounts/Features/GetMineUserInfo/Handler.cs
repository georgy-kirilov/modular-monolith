using Accounts.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OneOf;
using OneOf.Types;
using Shared.Authentication;
using Shared.Validation;
using Error = Shared.Validation.Error;

namespace Accounts.Features.GetMineUserInfo;

public sealed class Handler(
    IErrors _errors,
    UserManager<User> _userManager,
    IHttpContextAccessor _httpContextAccessor) : IHandler<None, OneOf<Response, Error[]>>
{
    public async Task<OneOf<Response, Error[]>> Handle(None request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext!.User.GetUserId().ToString();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return new[] { _errors.UserIdNotFound };
        }

        return new Response(user.Email, user.UserName);
    }
}
