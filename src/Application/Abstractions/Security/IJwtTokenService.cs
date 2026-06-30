namespace Template_net10.Application.Abstractions.Security;

/// <summary>Issues signed JWT access tokens. Implemented in Infrastructure.</summary>
public interface IJwtTokenService
{
    (string AccessToken, DateTime ExpiresAtUtc) GenerateToken(int userId, IEnumerable<string> permissions);
}
