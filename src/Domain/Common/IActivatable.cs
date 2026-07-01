namespace Template_net10.Domain.Common;

/// <summary>Implemented by <see cref="BaseEntity"/>. Use local <c>ActiveOnly()</c> when filtering active rows.</summary>
public interface IActivatable
{
    bool IsActive { get; }
}
