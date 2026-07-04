using System.Globalization;
using System.Reflection;
using ClosedXML.Excel;
using Template_net10.Application.Abstractions.Excel;

namespace Template_net10.Infrastructure.Services.Excel;

/// <summary>ClosedXML-backed <see cref="IExcelWriter"/>. Produces <c>.xlsx</c> bytes with a bold header row.</summary>
public sealed class ClosedXmlExcelWriter : IExcelWriter
{
    public byte[] Write<T>(
        IEnumerable<T> rows,
        string sheetName = "Sheet1",
        IReadOnlyList<ExcelColumn<T>>? columns = null)
    {
        columns ??= BuildColumnsFromProperties<T>();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(sheetName) ? "Sheet1" : sheetName);

        for (var c = 0; c < columns.Count; c++)
        {
            worksheet.Cell(1, c + 1).Value = columns[c].Header;
        }

        worksheet.Row(1).Style.Font.Bold = true;

        var rowIndex = 2;
        foreach (var row in rows)
        {
            for (var c = 0; c < columns.Count; c++)
            {
                SetCell(worksheet.Cell(rowIndex, c + 1), columns[c].Value(row));
            }

            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void SetCell(IXLCell cell, object? value)
    {
        switch (value)
        {
            case null:
                return;
            case string s:
                cell.Value = s;
                break;
            case bool b:
                cell.Value = b;
                break;
            case DateTime dt:
                cell.Value = dt;
                break;
            case DateOnly d:
                cell.Value = d.ToDateTime(TimeOnly.MinValue);
                break;
            case byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal:
                cell.Value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                break;
            default:
                cell.Value = value.ToString();
                break;
        }
    }

    private static IReadOnlyList<ExcelColumn<T>> BuildColumnsFromProperties<T>()
        => typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .Select(p => new ExcelColumn<T>(p.Name, x => p.GetValue(x)))
            .ToArray();
}
