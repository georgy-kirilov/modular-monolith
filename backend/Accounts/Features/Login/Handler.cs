using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using MassTransit;
using Shared;
using Shared.Authentication;
using Shared.Validation;
using Accounts.Database.Entities;
using Accounts.Settings;
using Accounts.Database;
using Accounts.Services;

namespace Accounts.Features.Login;

public sealed class Handler(
    IErrors _errors,
    Validator _validator,
    TimeProvider _timeProvider,
    UserManager<User> _userManager,
    AccountsDbContext _dbContext,
    JwtSettings _jwtSettings,
    AccountSettings _accountSettings,
    IPublishEndpoint _publishEndpoint,
    IHttpContextAccessor _httpContextAccessor) : IHandler<Request, OneOf<Response, Error[]>>
{
    public async Task<OneOf<Response, Error[]>> Handle(Request request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.GetErrors();
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return new[] { _errors.InvalidLoginCredentials };
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return new[] { _errors.InvalidLoginCredentials };
        }

        if (_accountSettings.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            await _publishEndpoint.Publish(new SendAccountConfirmationEmail(user.Id), cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new[] { _errors.EmailConfirmationRequired };
        }

        var jwtToken = GenerateJwtToken(user);

        if (request.StoreJwtInCookie)
        {
            AppendJwtAuthCookie(jwtToken);
        }

        return new Response(jwtToken, _jwtSettings.LifetimeInSeconds);
    }

    public string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var securityToken = new JwtSecurityToken
        (
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            expires: DateTime.UtcNow.AddSeconds(_jwtSettings.LifetimeInSeconds),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
            claims:
            [
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            ]
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public void AppendJwtAuthCookie(string jwtToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = _timeProvider.GetUtcNow().AddSeconds(_jwtSettings.LifetimeInSeconds)
        };

        _httpContextAccessor?.HttpContext?.Response.Cookies.Append(
            JwtAuthConstants.Cookie,
            jwtToken,
            cookieOptions);
    }
}
