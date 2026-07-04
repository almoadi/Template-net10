using Template_net10.Application.Abstractions.Security;

namespace Template_net10.Application.Common.Facades;

/// <summary>
/// Laravel-style static <c>Hash</c> facade. Proxies to the request-scoped <see cref="IPasswordHasher"/>
/// resolved through a delegate set at startup (<see cref="SetResolver"/>). Prefer injecting
/// <see cref="IPasswordHasher"/> in handlers/services; use this facade for Laravel-like ergonomics
/// (<c>Hash.Make(...)</c>, <c>Hash.Check(...)</c>). Only valid inside an HTTP request scope.
/// </summary>
public static class Hash
{
    private static Func<IPasswordHasher>? _resolver;

    /// <summary>Wires the facade to the DI container. Called once during application startup.</summary>
    public static void SetResolver(Func<IPasswordHasher> resolver) => _resolver = resolver;

    private static IPasswordHasher Instance => (_resolver ?? throw new InvalidOperationException(
        "Hash facade is not initialized. Call app.UseFacades() during startup."))();

    /// <summary>Hashes a value. (Laravel: <c>Hash::make</c>)</summary>
    public static string Make(string value) => Instance.Hash(value);

    /// <summary>Verifies a plain value against a previously computed hash. (Laravel: <c>Hash::check</c>)</summary>
    public static bool Check(string value, string hashedValue) => Instance.Verify(hashedValue, value);
}
