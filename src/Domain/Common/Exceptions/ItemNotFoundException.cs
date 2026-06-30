namespace Template_net10.Domain.Common.Exceptions;

/// <summary>Thrown when a requested entity does not exist. Maps to HTTP 404.</summary>
public sealed class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message)
    {
    }
}
