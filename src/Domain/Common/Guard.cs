using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Domain.Common;

/// <summary>
/// Guard clauses for enforcing domain invariants. Each check throws <see cref="BadRequestException"/>
/// (HTTP 400) when violated and returns the validated value, so it reads cleanly inside entity
/// constructors and behaviour methods: <c>_name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));</c>.
/// </summary>
public static class Guard
{
    public static T AgainstNull<T>(T? value, string name) where T : class
        => value ?? throw new BadRequestException($"{name} is required.");

    public static string AgainstNullOrEmpty(string? value, string name)
        => string.IsNullOrEmpty(value) ? throw new BadRequestException($"{name} is required.") : value;

    public static string AgainstNullOrWhiteSpace(string? value, string name)
        => string.IsNullOrWhiteSpace(value) ? throw new BadRequestException($"{name} is required.") : value;

    public static int AgainstNegative(int value, string name)
        => value < 0 ? throw new BadRequestException($"{name} must not be negative.") : value;

    public static decimal AgainstNegative(decimal value, string name)
        => value < 0 ? throw new BadRequestException($"{name} must not be negative.") : value;

    public static int AgainstNegativeOrZero(int value, string name)
        => value <= 0 ? throw new BadRequestException($"{name} must be greater than zero.") : value;

    public static decimal AgainstNegativeOrZero(decimal value, string name)
        => value <= 0 ? throw new BadRequestException($"{name} must be greater than zero.") : value;

    public static string AgainstMaxLength(string value, int maxLength, string name)
        => value.Length > maxLength
            ? throw new BadRequestException($"{name} must not exceed {maxLength} characters.")
            : value;

    /// <summary>Throws when <paramref name="invalidCondition"/> is true.</summary>
    public static void Against(bool invalidCondition, string message)
    {
        if (invalidCondition)
        {
            throw new BadRequestException(message);
        }
    }
}
