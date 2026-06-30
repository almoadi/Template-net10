namespace Template_net10.Application.Abstractions.Security;

/// <summary>Resolves the identity of the caller for the current request (from the JWT).</summary>
public interface ICurrentUserService
{
    int? UserId { get; }

    bool IsAuthenticated { get; }
}
