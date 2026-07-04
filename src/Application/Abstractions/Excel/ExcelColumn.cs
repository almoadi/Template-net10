namespace Template_net10.Application.Abstractions.Excel;

/// <summary>
/// A single spreadsheet column: a header plus a selector that extracts the cell value from a row.
/// Use with <see cref="IExcelWriter"/> to control column order, headers, and projected values.
/// </summary>
public sealed class ExcelColumn<T>
{
    public ExcelColumn(string header, Func<T, object?> value)
    {
        Header = header;
        Value = value;
    }

    public string Header { get; }

    public Func<T, object?> Value { get; }
}
