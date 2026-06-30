using Template_net10.Application.Auth;
using Template_net10.Application.Auth.Authentication;

namespace Template_net10.Application.Abstractions.Security;

/// <summary>
/// Laravel-style authentication facade contract. Backed by the request's JWT for identity/abilities
/// and by the database for credential checks and token issuance. Resolve via DI as <see cref="IAuth"/>
/// or use the static <c>Auth</c> facade (which proxies to this within the current request scope).
/// </summary>
public interface IAuth
{
    /// <summary>Id of the authenticated user, or <c>null</c> when unauthenticated. (Laravel: <c>Auth::id()</c>)</summary>
    int? Id { get; }

    /// <summary>True when a user is authenticated. (Laravel: <c>Auth::check()</c>)</summary>
    bool Check { get; }

    /// <summary>True when no user is authenticated. (Laravel: <c>Auth::guest()</c>)</summary>
    bool Guest { get; }

    /// <summary>Role names carried by the current token.</summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>Permission codes carried by the current token.</summary>
    IReadOnlyList<string> Permissions { get; }

    /// <summary>True when the current user has the given role.</summary>
    bool HasRole(string role);

    /// <summary>True when the current user holds the given permission. (Laravel: <c>$user->can()</c>)</summary>
    bool Can(string permission);

    /// <summary>Loads the authenticated user from the database, or <c>null</c>. (Laravel: <c>Auth::user()</c>)</summary>
    Task<CurrentUserDto?> User(CancellationToken cancellationToken = default);

    /// <summary>Verifies credentials and issues a JWT on success, else <c>null</c>. (Laravel: <c>Auth::attempt()</c>)</summary>
    Task<AuthTokenDto?> Attempt(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Verifies credentials without issuing a token. (Laravel: <c>Auth::validate()</c>)</summary>
    Task<bool> Validate(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Rotates a session's refresh token and issues a fresh access token, or <c>null</c> when invalid.</summary>
    Task<AuthTokenDto?> Refresh(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>Revokes the session backing the given refresh token. Returns false when no session matches.</summary>
    Task<bool> Logout(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>Revokes every active session of the current user. Returns the number revoked.</summary>
    Task<int> LogoutAll(CancellationToken cancellationToken = default);
}
