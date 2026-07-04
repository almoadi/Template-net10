using System.Diagnostics.CodeAnalysis;
using Template_net10.Application.Common.Support;

namespace Template_net10.Application.Common.Extensions;

/// <summary>Instance-style string helpers that delegate to <see cref="Str"/> for fluent call sites.</summary>
public static class StringExtensions
{
    /// <summary>True when the string is non-null and not whitespace.</summary>
    public static bool HasValue([NotNullWhen(true)] this string? value) => !string.IsNullOrWhiteSpace(value);

    /// <summary>True when the string is null, empty, or whitespace.</summary>
    public static bool IsBlank(this string? value) => string.IsNullOrWhiteSpace(value);

    public static string Slugify(this string value, char separator = '-') => Str.Slug(value, separator);

    public static string Truncate(this string value, int limit = 100, string end = "…") => Str.Limit(value, limit, end);

    public static string Mask(this string value, int visible = 4, char mask = '*') => Str.Mask(value, visible, mask);

    public static string ToSnakeCase(this string value) => Str.Snake(value);

    public static string ToKebabCase(this string value) => Str.Kebab(value);

    public static string ToCamelCase(this string value) => Str.Camel(value);

    public static string ToStudlyCase(this string value) => Str.Studly(value);
}
