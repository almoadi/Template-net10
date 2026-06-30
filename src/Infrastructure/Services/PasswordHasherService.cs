using Microsoft.AspNetCore.Identity;
using Template_net10.Application.Abstractions.Security;

namespace Template_net10.Infrastructure.Services;

/// <summary>ASP.NET Core Identity PBKDF2-based password hashing behind the Application abstraction.</summary>
public sealed class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();
    private static readonly object User = new();

    public string Hash(string password) => _hasher.HashPassword(User, password);

    public bool Verify(string hash, string password)
        => _hasher.VerifyHashedPassword(User, hash, password) != PasswordVerificationResult.Failed;
}
