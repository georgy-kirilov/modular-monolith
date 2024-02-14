namespace Accounts.Features.Login;

public sealed record Request
(
    string Email,
    string Password,
    bool StoreJwtInCookie
);

public sealed record Response(string Token, int LifetimeInSeconds);
