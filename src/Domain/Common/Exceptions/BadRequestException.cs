namespace Template_net10.Domain.Common.Exceptions;

/// <summary>Thrown when a request is well-formed but violates a business rule. Maps to HTTP 400.</summary>
public sealed class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
