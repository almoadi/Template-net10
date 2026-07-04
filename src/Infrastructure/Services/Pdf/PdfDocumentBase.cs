using QuestPDF.Infrastructure;

namespace Template_net10.Infrastructure.Services.Pdf;

/// <summary>
/// Convenience base for custom PDF designs. Implement <see cref="Compose"/> with QuestPDF's fluent
/// API and render the document via <see cref="IPdfRenderer"/>. Override <see cref="GetMetadata"/> to
/// customize document metadata.
/// </summary>
public abstract class PdfDocumentBase : IDocument
{
    public virtual DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public abstract void Compose(IDocumentContainer container);
}
