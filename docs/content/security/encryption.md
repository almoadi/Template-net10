# Encryption

Symmetric, authenticated encryption for short strings — connection secrets, tokens, or PII fields —
the .NET analog of Laravel's `Crypt`. The default driver uses **AES-GCM**.

## IEncryptor

```csharp
public interface IEncryptor
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
```

The Base64 payload is `nonce | tag | ciphertext`, so every call produces a unique ciphertext and
tampering is detected on decrypt.

## Injecting the service

```csharp
public sealed class StoreApiKeyHandler(IEncryptor encryptor, IApplicationDbContext context)
{
    public async Task Handle(StoreApiKeyCommand command, CancellationToken ct)
    {
        var protectedKey = encryptor.Encrypt(command.ApiKey);
        // persist protectedKey; decrypt on read
    }
}
```

## Crypt facade

```csharp
var cipher = Crypt.Encrypt("secret");
var plain  = Crypt.Decrypt(cipher);
```

## Key

Set `Encryption:Key` in `config/encryption.json` (or user-secrets / env vars). A Base64-encoded
32-byte value is used directly; any other non-empty string is hashed to a 256-bit key.

```bash
openssl rand -base64 32
```

> Never commit real keys. Supply them via user-secrets in development and environment variables or a
> vault in production.

## Related

- [Encryption Configuration](/docs/configuration/encryption)
- [Hashing](/docs/security/hashing)
