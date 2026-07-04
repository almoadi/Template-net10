using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Encryption;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services.Encryption;

/// <summary>
/// AES-GCM (authenticated) implementation of <see cref="IEncryptor"/>. The emitted Base64 payload is
/// <c>nonce | tag | ciphertext</c>. The key comes from <see cref="EncryptionOptions.Key"/>: a Base64
/// 32-byte value is used directly; any other non-empty string is hashed (SHA-256) to a 256-bit key.
/// </summary>
public sealed class AesEncryptor : IEncryptor
{
    private const int NonceSize = 12; // AesGcm.NonceByteSizes.MaxSize
    private const int TagSize = 16;   // AesGcm.TagByteSizes.MaxSize

    private readonly byte[] _key;

    public AesEncryptor(IOptions<EncryptionOptions> options) => _key = DeriveKey(options.Value.Key);

    public string Encrypt(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText);

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var cipher = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plainBytes, cipher, tag);

        var payload = new byte[NonceSize + TagSize + cipher.Length];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, payload, NonceSize, TagSize);
        Buffer.BlockCopy(cipher, 0, payload, NonceSize + TagSize, cipher.Length);

        return Convert.ToBase64String(payload);
    }

    public string Decrypt(string cipherText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cipherText);

        var payload = Convert.FromBase64String(cipherText);
        if (payload.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("Ciphertext payload is malformed.");
        }

        var nonce = payload.AsSpan(0, NonceSize);
        var tag = payload.AsSpan(NonceSize, TagSize);
        var cipher = payload.AsSpan(NonceSize + TagSize);
        var plainBytes = new byte[cipher.Length];

        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, cipher, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    private static byte[] DeriveKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "Encryption:Key is not set. Add a key to config/encryption.json (or user-secrets). " +
                "Generate one with: openssl rand -base64 32");
        }

        if (TryDecodeBase64(key, out var raw) && raw.Length == 32)
        {
            return raw;
        }

        return SHA256.HashData(Encoding.UTF8.GetBytes(key));
    }

    private static bool TryDecodeBase64(string value, out byte[] bytes)
    {
        bytes = [];
        Span<byte> buffer = new byte[value.Length];
        if (Convert.TryFromBase64String(value, buffer, out var written))
        {
            bytes = buffer[..written].ToArray();
            return true;
        }

        return false;
    }
}
