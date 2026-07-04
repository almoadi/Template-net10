using System.Text.RegularExpressions;

namespace Template_net10.Application.Common.Support;

/// <summary>Case-conversion and affix helpers for <see cref="Str"/> (Laravel: <c>Str::snake/camel/studly/...</c>).</summary>
public static partial class Str
{
    /// <summary>Converts to <c>snake_case</c> (Laravel: <c>Str::snake</c>).</summary>
    public static string Snake(string value, char separator = '_') => ToDelimited(value, separator);

    /// <summary>Converts to <c>kebab-case</c> (Laravel: <c>Str::kebab</c>).</summary>
    public static string Kebab(string value) => ToDelimited(value, '-');

    /// <summary>Converts to <c>StudlyCase</c> / PascalCase (Laravel: <c>Str::studly</c>).</summary>
    public static string Studly(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var spaced = CamelBoundaryRegex().Replace(value, "$1 $2");
        var words = NonAlphanumericRegex().Split(spaced).Where(w => w.Length > 0);
        return string.Concat(words.Select(w => UcFirst(w.ToLowerInvariant())));
    }

    /// <summary>Converts to <c>camelCase</c> (Laravel: <c>Str::camel</c>).</summary>
    public static string Camel(string value)
    {
        var studly = Studly(value);
        return studly.Length == 0 ? studly : char.ToLowerInvariant(studly[0]) + studly[1..];
    }

    /// <summary>Capitalizes each whitespace-separated word (Laravel: <c>Str::title</c>).</summary>
    public static string Title(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value ?? string.Empty;
        }

        var words = value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', words.Select(w => UcFirst(w.ToLowerInvariant())));
    }

    /// <summary>Case-insensitive substring test (Laravel: <c>Str::contains</c>).</summary>
    public static bool Contains(string? haystack, string needle)
        => haystack is not null && haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);

    /// <summary>Ensures the string begins with <paramref name="prefix"/> exactly once (Laravel: <c>Str::start</c>).</summary>
    public static string Start(string value, string prefix)
    {
        value ??= string.Empty;
        return value.StartsWith(prefix, StringComparison.Ordinal) ? value : prefix + value;
    }

    /// <summary>Ensures the string ends with <paramref name="suffix"/> exactly once (Laravel: <c>Str::finish</c>).</summary>
    public static string Finish(string value, string suffix)
    {
        value ??= string.Empty;
        return value.EndsWith(suffix, StringComparison.Ordinal) ? value : value + suffix;
    }

    private static string ToDelimited(string value, char delimiter)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var boundary = CamelBoundaryRegex().Replace(value, $"$1{delimiter}$2");
        var cleaned = NonAlphanumericRegex().Replace(boundary, delimiter.ToString());
        return cleaned.Trim(delimiter).ToLowerInvariant();
    }

    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex CamelBoundaryRegex();

    [GeneratedRegex(@"[^a-zA-Z0-9]+")]
    private static partial Regex NonAlphanumericRegex();
}
