using Template_net10.Application.Abstractions.Encryption;

namespace Template_net10.Application.Common.Facades;

/// <summary>
/// Laravel-style static <c>Crypt</c> facade. Proxies to the request-scoped <see cref="IEncryptor"/>
/// resolved through a delegate set at startup (<see cref="SetResolver"/>). Prefer injecting
/// <see cref="IEncryptor"/> in handlers/services; use this facade for Laravel-like ergonomics
/// (<c>Crypt.Encrypt(...)</c>). Only valid inside an HTTP request scope.
/// </summary>
public static class Crypt
{
    private static Func<IEncryptor>? _resolver;

    /// <summary>Wires the facade to the DI container. Called once during application startup.</summary>
    public static void SetResolver(Func<IEncryptor> resolver) => _resolver = resolver;

    private static IEncryptor Instance => (_resolver ?? throw new InvalidOperationException(
        "Crypt facade is not initialized. Call app.UseFacades() during startup."))();

    public static string Encrypt(string plainText) => Instance.Encrypt(plainText);

    public static string Decrypt(string cipherText) => Instance.Decrypt(cipherText);
}
