namespace Template_net10.Infrastructure.Options;

/// <summary>Bound from the <c>Encryption</c> section (config/encryption.json).</summary>
public sealed class EncryptionOptions
{
    public const string SectionName = "Encryption";

    /// <summary>
    /// Application encryption key. Ideally a Base64-encoded 32-byte value
    /// (generate with <c>openssl rand -base64 32</c>). Any other non-empty string is accepted and
    /// hashed to a 256-bit key. Keep this out of source control — supply via user-secrets / env vars.
    /// </summary>
    public string Key { get; set; } = string.Empty;
}
