namespace Template_net10.Domain.Common.Exceptions;

/// <summary>Thrown when the caller is authenticated but not allowed to perform the action. Maps to HTTP 403.</summary>
public sealed class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "Forbidden access.") : base(message)
    {
    }
}
