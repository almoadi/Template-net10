namespace Template_net10.Domain.Auth.Enums;

/// <summary>
/// Supported external identity providers for social login (the Laravel Socialite analog).
/// Add a new value here and a matching driver in Infrastructure to support another provider.
/// </summary>
public enum SocialProvider
{
    /// <summary>Google sign-in (OAuth 2.0 / OpenID Connect).</summary>
    Google,

    /// <summary>Microsoft Entra ID (Azure AD) sign-in.</summary>
    Azure,
}
