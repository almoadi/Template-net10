namespace Template_net10.Application.Abstractions.Encryption;

/// <summary>
/// Symmetric encryption abstraction (Laravel: the <c>Crypt</c> facade). Encrypts/decrypts short
/// strings — connection secrets, tokens, PII fields — using an application key. The default driver
/// uses AES-GCM (authenticated encryption). Inject <see cref="IEncryptor"/> or use the static
/// <c>Crypt</c> facade.
/// </summary>
public interface IEncryptor
{
    /// <summary>Encrypts <paramref name="plainText"/> and returns a Base64 payload. (Laravel: <c>Crypt::encryptString</c>)</summary>
    string Encrypt(string plainText);

    /// <summary>Decrypts a payload produced by <see cref="Encrypt"/>. (Laravel: <c>Crypt::decryptString</c>)</summary>
    string Decrypt(string cipherText);
}
