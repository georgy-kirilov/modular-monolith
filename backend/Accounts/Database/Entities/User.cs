using Microsoft.AspNetCore.Identity;

namespace Accounts.Database.Entities;

public sealed class User : IdentityUser<Guid> { }
