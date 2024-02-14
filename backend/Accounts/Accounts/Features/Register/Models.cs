namespace Accounts.Features.Register;

public sealed record Request
(
    string Email,
    string Password,
    string ConfirmPassword
);

public sealed record Response(int EmailConfirmationThresholdInSeconds);
