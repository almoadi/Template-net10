namespace Template_net10.Domain.Common;

/// <summary>
/// Shared column/string length constants used by entity configurations and validators
/// so the database schema and validation rules never drift apart.
/// </summary>
public static class LengthConstants
{
    public const int L20 = 20;
    public const int L50 = 50;
    public const int L100 = 100;
    public const int L255 = 255;
    public const int L500 = 500;
    public const int L1000 = 1000;
}
