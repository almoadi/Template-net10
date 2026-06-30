namespace Template_net10.Application.Abstractions.Localization;

/// <summary>
/// Keys for user-facing messages. The concrete text lives in the localization provider,
/// never inline in handlers (Golden Rule: no hardcoded user-facing text).
/// </summary>
public enum Resource
{
    OperationSucceeded,
    UserCreated,
    UserUpdated,
    UserNotFound,
    PhoneAlreadyExists,
    EmailAlreadyExists,
    InvalidCredentials,
    InvalidRefreshToken,
    LoggedOut,
    AllSessionsRevoked,
    RoleCreated,
    RoleNotFound,
    RolesAssigned,
}
