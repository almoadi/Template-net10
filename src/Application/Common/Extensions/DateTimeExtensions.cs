using System.Globalization;

namespace Template_net10.Application.Common.Extensions;

/// <summary>Convenience helpers for <see cref="DateTime"/> boundaries and Unix/ISO conversions.</summary>
public static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime value) => value.Date;

    public static DateTime EndOfDay(this DateTime value) => value.Date.AddDays(1).AddTicks(-1);

    public static DateTime StartOfMonth(this DateTime value)
        => new(value.Year, value.Month, 1, 0, 0, 0, value.Kind);

    public static DateTime EndOfMonth(this DateTime value)
        => value.StartOfMonth().AddMonths(1).AddTicks(-1);

    /// <summary>Seconds since the Unix epoch (converts to UTC first).</summary>
    public static long ToUnixTimeSeconds(this DateTime value)
        => new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeSeconds();

    /// <summary>Round-trip ("O") ISO-8601 representation in UTC.</summary>
    public static string ToIso8601(this DateTime value)
        => value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    /// <summary>Converts seconds since the Unix epoch to a UTC <see cref="DateTime"/>.</summary>
    public static DateTime FromUnixTimeSeconds(long seconds)
        => DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
}
