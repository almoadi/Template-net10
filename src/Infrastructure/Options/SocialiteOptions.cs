namespace Template_net10.Infrastructure.Options;

/// <summary>Bound from the <c>Socialite</c> section (config/socialite.json). Social login is opt-in per provider.</summary>
public sealed class SocialiteOptions
{
    public const string SectionName = "Socialite";

    public SocialProviderOptions Google { get; set; } = new();

    public SocialProviderOptions Azure { get; set; } = new();
}

/// <summary>Per-provider social login settings.</summary>
public sealed class SocialProviderOptions
{
    /// <summary>When false (default) this provider is rejected even if a driver exists.</summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Optional OAuth client id issued by the provider. Used for documentation and, where supported,
    /// to validate that the incoming token was minted for this application (audience check).
    /// </summary>
    public string? ClientId { get; set; }
}
