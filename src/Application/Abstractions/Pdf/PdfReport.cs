namespace Template_net10.Application.Abstractions.Pdf;

/// <summary>
/// A simple tabular report model rendered to PDF by <see cref="IPdfGenerator"/>. Covers the common
/// "export a list/report as a PDF table" need — title, optional subtitle, columns, rows, and footer.
/// </summary>
public sealed class PdfReport
{
    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    /// <summary>Column headers. Must contain at least one entry.</summary>
    public IReadOnlyList<string> Columns { get; set; } = [];

    /// <summary>Rows of cell text. Short rows are padded; extra cells are ignored.</summary>
    public IReadOnlyList<IReadOnlyList<string>> Rows { get; set; } = [];

    /// <summary>Optional note rendered in the page footer alongside the page number.</summary>
    public string? FooterNote { get; set; }

    /// <summary>Branding and layout options for the rendered table report.</summary>
    public PdfDesign Design { get; set; } = new();
}
