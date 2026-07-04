namespace Template_net10.Application.Abstractions.Excel;

/// <summary>
/// Writes tabular data to an <c>.xlsx</c> workbook (implemented with ClosedXML in Infrastructure).
/// Inject this in handlers to produce spreadsheet exports; return the bytes as a file download.
/// </summary>
public interface IExcelWriter
{
    /// <summary>
    /// Renders <paramref name="rows"/> to a single-sheet workbook and returns the file bytes.
    /// When <paramref name="columns"/> is <c>null</c>, one column per public property is generated.
    /// </summary>
    byte[] Write<T>(
        IEnumerable<T> rows,
        string sheetName = "Sheet1",
        IReadOnlyList<ExcelColumn<T>>? columns = null);
}
