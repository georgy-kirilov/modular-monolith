using Accounts.Database.Entities;
using Accounts.Settings;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using OneOf;
using Shared.Validation;

namespace Accounts.Features.Register;

public sealed class Handler(
    Validator _validator,
    UserManager<User> _userManager,
    IPublishEndpoint _publishEndpoint,
    AccountSettings _accountSettings) : IHandler<Request, OneOf<Response, Error[]>>
{
    public async Task<OneOf<Response, Error[]>> Handle(Request request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorsArray();
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email
        };

        var identityResult = await _userManager.CreateAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            return identityResult
                .Errors
                .Select(err => new Error(err.Code, err.Description))
                .ToArray();
        }

        await _publishEndpoint.Publish(new UserAccountCreated.Message(user), cancellationToken);

        return new Response(_accountSettings.Email.EmailConfirmationThresholdInSeconds);
    }
}
