namespace Template_net10.Application.Abstractions.Excel;

/// <summary>
/// Reads rows from an <c>.xlsx</c> workbook into typed objects (implemented with ClosedXML).
/// The first row is treated as headers; header text is matched to public property names
/// (case-insensitive). Use for spreadsheet imports/uploads.
/// </summary>
public interface IExcelReader
{
    /// <summary>
    /// Reads the given worksheet (or the first one) into a list of <typeparamref name="T"/>.
    /// Cells are converted to each property's type; unparseable/blank cells are left at default.
    /// </summary>
    IReadOnlyList<T> Read<T>(Stream stream, string? sheetName = null) where T : new();
}
