namespace Template_net10.Domain.Common.Exceptions;

/// <summary>Thrown when a caller exceeds an allowed rate. Maps to HTTP 429.</summary>
public sealed class TooManyRequestsException : Exception
{
    public TooManyRequestsException(string message = "Too many requests.") : base(message)
    {
    }
}
