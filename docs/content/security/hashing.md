# Hashing

One-way hashing for passwords and other secrets — the .NET analog of Laravel's `Hash`. Backed by
ASP.NET Core Identity's PBKDF2 hasher via `IPasswordHasher`.

## Hash facade

```csharp
var hash = Hash.Make("plain-text-password");

if (Hash.Check("plain-text-password", hash))
{
    // matches
}
```

## Injecting the service

```csharp
public sealed class ResetPasswordHandler(IPasswordHasher hasher)
{
    public void Handle(ResetPasswordCommand command)
    {
        var hash = hasher.Hash(command.NewPassword);
        // persist hash
    }
}
```

## Hashing vs. Encryption

| Use | Reversible? | Helper |
|-----|-------------|--------|
| Passwords, secrets you only compare | No | [`Hash`](/docs/security/hashing) |
| Values you must read back | Yes | [`Crypt`](/docs/security/encryption) |

## Related

- [Encryption](/docs/security/encryption)
- [Authentication Overview](/docs/authentication/overview)
