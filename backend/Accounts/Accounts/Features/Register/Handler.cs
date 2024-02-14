using Microsoft.AspNetCore.Identity;
using Accounts.Database;
using Accounts.Database.Entities;
using Accounts.Services;
using Accounts.Settings;
using OneOf;
using MassTransit;
using Shared.Validation;

namespace Accounts.Features.Register;

public sealed class Handler(
    Validator _validator,
    UserManager<User> _userManager,
    IPublishEndpoint _publishEndpoint,
    AccountsDbContext _dbContext,
    AccountSettings _accountSettings) : IHandler<Request, OneOf<Response, Error[]>>
{
    public async Task<OneOf<Response, Error[]>> Handle(Request request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.GetErrors();
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var identityResult = await _userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            return identityResult.Errors.Select(err => new Error(err.Code, err.Description)).ToArray();
        }

        await _publishEndpoint.Publish(new SendAccountConfirmationEmail(user.Id), cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new Response(_accountSettings.Email.EmailConfirmationThresholdInSeconds);
    }
}
