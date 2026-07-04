using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Template_net10.Infrastructure.Services.Pdf;

/// <summary>
/// Escape hatch for fully custom PDF designs. Where <c>IPdfGenerator</c> renders the built-in
/// tabular report, this renders any QuestPDF layout — either a reusable <see cref="IDocument"/>
/// (recommended for real templates, e.g. an invoice) or an inline compose delegate. Because custom
/// layouts are a presentation concern, define <see cref="IDocument"/> implementations in Infrastructure.
/// </summary>
public interface IPdfRenderer
{
    /// <summary>Renders a reusable QuestPDF document (e.g. a custom invoice/report class).</summary>
    byte[] Render(IDocument document);

    /// <summary>Renders an inline QuestPDF layout without a dedicated document class.</summary>
    byte[] Render(Action<IDocumentContainer> compose);
}
