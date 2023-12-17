using Microsoft.AspNetCore.Identity;

namespace Accounts.Database.Entities;

public sealed class User : IdentityUser<Guid>
{
    public DateTimeOffset? LastEmailConfirmationSentAt { get; set; }

    public bool RequireEmailConfirmation(bool requireConfirmedEmail) => requireConfirmedEmail && !EmailConfirmed;

    public bool CanSendEmailConfirmation(DateTimeOffset now, int thresholdInSeconds)
    {
        if (LastEmailConfirmationSentAt is null)
        {
            return true;
        }

        return LastEmailConfirmationSentAt.Value.AddSeconds(thresholdInSeconds).UtcDateTime < now.UtcDateTime;
    }
}
