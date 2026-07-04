using System.Globalization;
using System.Reflection;
using ClosedXML.Excel;
using Template_net10.Application.Abstractions.Excel;

namespace Template_net10.Infrastructure.Services.Excel;

/// <summary>
/// ClosedXML-backed <see cref="IExcelReader"/>. Uses the first row as headers and maps each header
/// to a public settable property of <typeparamref name="T"/> (case-insensitive).
/// </summary>
public sealed class ClosedXmlExcelReader : IExcelReader
{
    public IReadOnlyList<T> Read<T>(Stream stream, string? sheetName = null) where T : new()
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = sheetName is null ? workbook.Worksheets.First() : workbook.Worksheet(sheetName);

        var rows = worksheet.RowsUsed().ToList();
        if (rows.Count < 2)
        {
            return [];
        }

        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        var columnMap = new List<(int Column, PropertyInfo Property)>();
        foreach (var headerCell in rows[0].CellsUsed())
        {
            var header = headerCell.GetString().Trim();
            if (properties.TryGetValue(header, out var property))
            {
                columnMap.Add((headerCell.Address.ColumnNumber, property));
            }
        }

        var results = new List<T>(rows.Count - 1);
        foreach (var row in rows.Skip(1))
        {
            var item = new T();
            foreach (var (column, property) in columnMap)
            {
                var text = row.Cell(column).GetString();
                var value = ConvertTo(text, property.PropertyType);
                if (value is not null)
                {
                    property.SetValue(item, value);
                }
            }

            results.Add(item);
        }

        return results;
    }

    private static object? ConvertTo(string text, Type targetType)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            if (type == typeof(string))
            {
                return text;
            }

            if (type == typeof(Guid))
            {
                return Guid.Parse(text);
            }

            if (type == typeof(DateOnly))
            {
                return DateOnly.Parse(text, CultureInfo.InvariantCulture);
            }

            if (type == typeof(DateTime))
            {
                return DateTime.Parse(text, CultureInfo.InvariantCulture);
            }

            if (type.IsEnum)
            {
                return Enum.Parse(type, text, ignoreCase: true);
            }

            return Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException or ArgumentException)
        {
            return null;
        }
    }
}
