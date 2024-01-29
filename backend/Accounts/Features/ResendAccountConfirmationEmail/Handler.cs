using Accounts.Database;
using Accounts.Database.Entities;
using Accounts.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using OneOf;
using OneOf.Types;
using Shared.Validation;
using Error = Shared.Validation.Error;

namespace Accounts.Features.ResendAccountConfirmationEmail;

public sealed class Handler(
    UserManager<User> _userManager,
    IErrors _errors,
    IPublishEndpoint _publishEndpoint,
    AccountsDbContext _dbContext) : IHandler<Request, OneOf<Success, Error[]>>
{
    public async Task<OneOf<Success, Error[]>> Handle(Request request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return new[] { _errors.UserEmailNotFound };
        }

        await _publishEndpoint.Publish(new SendAccountConfirmationEmail(user.Id), cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
