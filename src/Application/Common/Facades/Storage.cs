using Template_net10.Application.Abstractions.Storage;

namespace Template_net10.Application.Common.Facades;

/// <summary>
/// Laravel-style static <c>Storage</c> facade. Proxies to the request-scoped <see cref="IStorage"/>
/// resolved through a delegate set at startup (<see cref="SetResolver"/>). Prefer injecting
/// <see cref="IStorage"/> in handlers/services; use this facade for Laravel-like ergonomics
/// (<c>Storage.Put(...)</c>, <c>Storage.Url(...)</c>). Only valid inside an HTTP request scope.
/// </summary>
public static class Storage
{
    private static Func<IStorage>? _resolver;

    /// <summary>Wires the facade to the DI container. Called once during application startup.</summary>
    public static void SetResolver(Func<IStorage> resolver) => _resolver = resolver;

    private static IStorage Instance => (_resolver ?? throw new InvalidOperationException(
        "Storage facade is not initialized. Call app.UseFacades() during startup."))();

    public static Task<string> Put(string path, Stream content, CancellationToken cancellationToken = default)
        => Instance.PutAsync(path, content, cancellationToken);

    public static Task<string> PutBytes(string path, byte[] content, CancellationToken cancellationToken = default)
        => Instance.PutBytesAsync(path, content, cancellationToken);

    public static Task<Stream?> Get(string path, CancellationToken cancellationToken = default)
        => Instance.GetAsync(path, cancellationToken);

    public static Task<bool> Exists(string path, CancellationToken cancellationToken = default)
        => Instance.ExistsAsync(path, cancellationToken);

    public static Task Delete(string path, CancellationToken cancellationToken = default)
        => Instance.DeleteAsync(path, cancellationToken);

    public static string Url(string path) => Instance.Url(path);
}
