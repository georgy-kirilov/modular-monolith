using System.Security.Claims;

namespace Shared.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));
        return Guid.Parse(userId);
    }

    public static string GetUsername(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        var username = user.FindFirstValue(ClaimTypes.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
        return username;
    }
}
