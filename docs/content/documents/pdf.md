# PDF

Generate PDF documents — backed by [QuestPDF](https://www.questpdf.com/). Inject `IPdfGenerator` in
handlers to produce printable tabular reports.

## Generating a report

```csharp
public sealed class UsersPdfHandler(IApplicationDbContext context, IPdfGenerator pdf)
{
    public async Task<byte[]> Handle(UsersPdfQuery query, CancellationToken ct)
    {
        var users = await context.Users.AsNoTracking()
            .Select(u => new { u.NameEn, u.Email })
            .ToListAsync(ct);

        return pdf.Render(new PdfReport
        {
            Title = "Users",
            Subtitle = $"Generated {DateTime.UtcNow:yyyy-MM-dd}",
            Columns = ["Name", "Email"],
            Rows = users.Select(u => (IReadOnlyList<string>)[u.NameEn, u.Email]).ToList(),
            FooterNote = "Confidential",
        });
    }
}
```

Return the bytes from a controller as a file:

```csharp
return File(bytes, "application/pdf", "users.pdf");
```

The generated document is A4 with a title/subtitle header, a bordered table, and a page-number footer.
Short rows are padded to the column count.

## Branding the built-in report

Pass a `PdfDesign` to control page size, colors, fonts, logo, and column widths — all without
touching the PDF engine:

```csharp
return pdf.Render(new PdfReport
{
    Title = "Sales",
    Columns = ["Product", "Qty", "Total"],
    Rows = rows,
    Design = new PdfDesign
    {
        PageSize = PdfPageSize.A4,
        Landscape = true,
        AccentColorHex = "#2563EB",   // title + table header color
        LogoPng = logoBytes,          // optional header logo
        ColumnWidths = [3f, 1f, 1f],  // relative widths
        ZebraRows = true,
        FontFamily = "Arial",
        BaseFontSize = 10f,
    },
});
```

## Fully custom designs

When the table report isn't enough, render **any** QuestPDF layout with `IPdfRenderer`. Because a
bespoke layout is a presentation concern, define the document in Infrastructure.

Create a reusable document (extend `PdfDocumentBase`):

```csharp
public sealed class InvoiceDocument(Invoice invoice) : PdfDocumentBase
{
    public override void Compose(IDocumentContainer container) =>
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.Header().Text($"Invoice #{invoice.Number}").FontSize(22).Bold();
            page.Content().Table(table => { /* ...your layout... */ });
            page.Footer().AlignRight().Text(t => { t.Span("Page "); t.CurrentPageNumber(); });
        });
}

// render it
byte[] bytes = renderer.Render(new InvoiceDocument(invoice));
```

Or render an inline layout without a dedicated class:

```csharp
byte[] bytes = renderer.Render(container => container.Page(page =>
{
    page.Size(PageSizes.A4);
    page.Content().Text("Fully custom design");
}));
```

See the [QuestPDF documentation](https://www.questpdf.com/) for the full layout API.

## Licensing

QuestPDF ships under the **Community** license (free for individuals and companies under the revenue
threshold; a paid license is required above it). The license type is set once in `QuestPdfGenerator`.
Review the [QuestPDF license](https://www.questpdf.com/license/) for your use, and swap the
`IPdfGenerator` implementation if you need a different engine.

## Related

- [Excel](/docs/documents/excel)
- [File Storage](/docs/storage/overview)
