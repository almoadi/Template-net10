namespace Template_net10.Application.Abstractions.Pdf;

public enum PdfPageSize
{
    A3,
    A4,
    A5,
    Letter,
    Legal,
}

/// <summary>
/// Framework-agnostic design options for a <see cref="PdfReport"/>. Lets callers brand and lay out
/// the built-in table report (page size, colors, logo, fonts, column widths) without depending on the
/// underlying PDF engine. For fully bespoke layouts, use a custom document via <c>IPdfRenderer</c>.
/// </summary>
public sealed class PdfDesign
{
    public PdfPageSize PageSize { get; set; } = PdfPageSize.A4;

    public bool Landscape { get; set; }

    public float Margin { get; set; } = 30f;

    /// <summary>Optional font family name (must be available on the host). Defaults to the engine font.</summary>
    public string? FontFamily { get; set; }

    public float BaseFontSize { get; set; } = 10f;

    /// <summary>Accent color as hex (e.g. <c>#2563EB</c>) used for the title and table header. Optional.</summary>
    public string? AccentColorHex { get; set; }

    /// <summary>Optional PNG logo bytes rendered at the top of the header.</summary>
    public byte[]? LogoPng { get; set; }

    /// <summary>Optional relative column widths; index-aligned to <see cref="PdfReport.Columns"/>.</summary>
    public float[]? ColumnWidths { get; set; }

    /// <summary>Alternate a subtle background on every other data row.</summary>
    public bool ZebraRows { get; set; } = true;
}
