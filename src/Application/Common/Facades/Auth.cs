using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Auth;
using Template_net10.Application.Auth.Authentication;

namespace Template_net10.Application.Common.Facades;

/// <summary>
/// Laravel-style static <c>Auth</c> facade. Proxies every call to the request-scoped
/// <see cref="IAuth"/> resolved through a delegate set once at startup (<see cref="SetResolver"/>).
/// Prefer injecting <see cref="IAuth"/> in handlers/services; use this facade for Laravel-like
/// ergonomics (<c>Auth.Check</c>, <c>Auth.Id</c>, <c>Auth.User()</c>, <c>Auth.Attempt(...)</c>).
/// Only valid inside an HTTP request scope.
/// </summary>
public static class Auth
{
    private static Func<IAuth>? _resolver;

    /// <summary>Wires the facade to the DI container. Called once during application startup.</summary>
    public static void SetResolver(Func<IAuth> resolver) => _resolver = resolver;

    private static IAuth Instance => (_resolver ?? throw new InvalidOperationException(
        "Auth facade is not initialized. Call app.UseAuthFacade() during startup."))();

    public static int? Id => Instance.Id;

    public static bool Check => Instance.Check;

    public static bool Guest => Instance.Guest;

    public static IReadOnlyList<string> Roles => Instance.Roles;

    public static IReadOnlyList<string> Permissions => Instance.Permissions;

    public static bool HasRole(string role) => Instance.HasRole(role);

    public static bool Can(string permission) => Instance.Can(permission);

    public static Task<CurrentUserDto?> User(CancellationToken cancellationToken = default)
        => Instance.User(cancellationToken);

    public static Task<AuthTokenDto?> Attempt(
        string email, string password, CancellationToken cancellationToken = default)
        => Instance.Attempt(email, password, cancellationToken);

    public static Task<bool> Validate(
        string email, string password, CancellationToken cancellationToken = default)
        => Instance.Validate(email, password, cancellationToken);
}
