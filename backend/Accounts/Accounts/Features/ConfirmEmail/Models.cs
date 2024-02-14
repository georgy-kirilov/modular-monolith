namespace Accounts.Features.ConfirmEmail;

public sealed record Request(Guid UserId, string Token);
