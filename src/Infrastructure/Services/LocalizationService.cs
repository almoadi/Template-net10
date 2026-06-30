using Template_net10.Application.Abstractions.Localization;

namespace Template_net10.Infrastructure.Services;

/// <summary>
/// Minimal in-memory localization. Swap for a resx/DB-backed provider as needs grow;
/// handlers depend only on <see cref="ILocalizationService"/>.
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private static readonly IReadOnlyDictionary<Resource, string> Messages = new Dictionary<Resource, string>
    {
        [Resource.OperationSucceeded] = "Operation completed successfully.",
        [Resource.UserCreated] = "User created successfully.",
        [Resource.UserUpdated] = "User updated successfully.",
        [Resource.UserNotFound] = "User not found.",
        [Resource.PhoneAlreadyExists] = "A user with this phone number already exists.",
        [Resource.EmailAlreadyExists] = "A user with this email already exists.",
        [Resource.InvalidCredentials] = "Invalid email or password.",
        [Resource.RoleCreated] = "Role created successfully.",
        [Resource.RoleNotFound] = "Role not found.",
        [Resource.RolesAssigned] = "Roles assigned successfully.",
    };

    public string GetMessage(Resource resource)
        => Messages.TryGetValue(resource, out var message) ? message : resource.ToString();
}
