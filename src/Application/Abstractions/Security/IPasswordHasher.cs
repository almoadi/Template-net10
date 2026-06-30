namespace Template_net10.Application.Abstractions.Security;

/// <summary>Hashes and verifies user passwords. Implemented in Infrastructure.</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string hash, string password);
}
