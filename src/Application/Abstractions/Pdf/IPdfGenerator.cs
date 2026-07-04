namespace Template_net10.Application.Abstractions.Pdf;

/// <summary>
/// Generates PDF documents (implemented with QuestPDF in Infrastructure). Inject this in handlers to
/// produce printable reports/exports; return the bytes as a file download.
/// </summary>
public interface IPdfGenerator
{
    /// <summary>Renders a tabular <see cref="PdfReport"/> and returns the PDF file bytes.</summary>
    byte[] Render(PdfReport report);
}
