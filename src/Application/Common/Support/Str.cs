using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Template_net10.Application.Common.Support;

/// <summary>
/// Laravel-style <c>Str</c> string helpers. Pure, dependency-free static utilities for common
/// string operations (random tokens, slugs, truncation, masking). Use anywhere — no DI required.
/// </summary>
public static partial class Str
{
    private const string AlphanumericChars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <summary>Cryptographically-strong random alphanumeric string. (Laravel: <c>Str::random</c>)</summary>
    public static string Random(int length = 32)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        return RandomNumberGenerator.GetString(AlphanumericChars, length);
    }

    /// <summary>A new GUID as a 32-char lowercase hex string (no dashes). (Laravel: <c>Str::uuid</c>)</summary>
    public static string Uuid() => Guid.NewGuid().ToString("n");

    /// <summary>URL-friendly "slug" from arbitrary text. (Laravel: <c>Str::slug</c>)</summary>
    public static string Slug(string value, char separator = '-')
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant();
        normalized = NonWordRegex().Replace(normalized, separator.ToString());
        normalized = normalized.Trim(separator);
        return normalized;
    }

    /// <summary>Truncates to <paramref name="limit"/> characters, appending <paramref name="end"/>. (Laravel: <c>Str::limit</c>)</summary>
    public static string Limit(string value, int limit = 100, string end = "…")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= limit)
        {
            return value;
        }

        return string.Concat(value.AsSpan(0, limit).TrimEnd(), end);
    }

    /// <summary>Masks all but the last <paramref name="visible"/> characters (e.g. for secrets/PII). (Laravel: <c>Str::mask</c>)</summary>
    public static string Mask(string value, int visible = 4, char mask = '*')
    {
        if (string.IsNullOrEmpty(value) || value.Length <= visible)
        {
            return value;
        }

        return string.Concat(new string(mask, value.Length - visible), value.AsSpan(value.Length - visible));
    }

    /// <summary>Upper-cases the first character. (Laravel: <c>Str::ucfirst</c>)</summary>
    public static string UcFirst(string value)
        => string.IsNullOrEmpty(value) ? value : char.ToUpper(value[0], CultureInfo.InvariantCulture) + value[1..];

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonWordRegex();
}
