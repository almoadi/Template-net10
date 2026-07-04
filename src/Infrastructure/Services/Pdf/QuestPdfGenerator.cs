using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Template_net10.Application.Abstractions.Pdf;

namespace Template_net10.Infrastructure.Services.Pdf;

/// <summary>
/// QuestPDF-backed PDF generation. Implements <see cref="IPdfGenerator"/> (the built-in, branded
/// tabular report driven by <see cref="PdfReport"/> + <see cref="PdfDesign"/>) and
/// <see cref="IPdfRenderer"/> (an escape hatch for fully custom QuestPDF layouts). The QuestPDF
/// Community license is set once here.
/// </summary>
public sealed class QuestPdfGenerator : IPdfGenerator, IPdfRenderer
{
    static QuestPdfGenerator() => QuestPDF.Settings.License = LicenseType.Community;

    public byte[] Render(PdfReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        if (report.Columns.Count == 0)
        {
            throw new ArgumentException("A PDF report requires at least one column.", nameof(report));
        }

        var design = report.Design ?? new PdfDesign();
        var accent = ResolveColor(design.AccentColorHex);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(MapPageSize(design.PageSize, design.Landscape));
                page.Margin(design.Margin);
                page.DefaultTextStyle(style =>
                {
                    style = style.FontSize(design.BaseFontSize);
                    return string.IsNullOrWhiteSpace(design.FontFamily) ? style : style.FontFamily(design.FontFamily);
                });

                ComposeHeader(page, report, design, accent);
                ComposeTable(page, report, design, accent);
                ComposeFooter(page, report);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] Render(IDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        return document.GeneratePdf();
    }

    public byte[] Render(Action<IDocumentContainer> compose)
    {
        ArgumentNullException.ThrowIfNull(compose);
        return Document.Create(compose).GeneratePdf();
    }

    private static void ComposeHeader(PageDescriptor page, PdfReport report, PdfDesign design, Color? accent)
    {
        page.Header().Row(row =>
        {
            var hasLogo = design.LogoPng is { Length: > 0 };
            if (hasLogo)
            {
                row.ConstantItem(70).AlignMiddle().Image(design.LogoPng!);
            }

            row.RelativeItem().PaddingLeft(hasLogo ? 10 : 0).Column(column =>
            {
                var title = column.Item().Text(report.Title).FontSize(18).SemiBold();
                if (accent is not null)
                {
                    title.FontColor(accent.Value);
                }

                if (!string.IsNullOrWhiteSpace(report.Subtitle))
                {
                    column.Item().Text(report.Subtitle).FontSize(11).FontColor(Colors.Grey.Darken1);
                }
            });
        });
    }

    private static void ComposeTable(PageDescriptor page, PdfReport report, PdfDesign design, Color? accent)
    {
        page.Content().PaddingVertical(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                for (var i = 0; i < report.Columns.Count; i++)
                {
                    var width = design.ColumnWidths is { } widths && i < widths.Length ? widths[i] : 1f;
                    columns.RelativeColumn(width);
                }
            });

            table.Header(header =>
            {
                foreach (var heading in report.Columns)
                {
                    var cell = header.Cell().Background(accent ?? Colors.Grey.Lighten3).Padding(4);
                    var text = cell.Text(heading).SemiBold();
                    if (accent is not null)
                    {
                        text.FontColor(Colors.White);
                    }
                }
            });

            var rowIndex = 0;
            foreach (var row in report.Rows)
            {
                var striped = design.ZebraRows && rowIndex % 2 == 1;
                for (var i = 0; i < report.Columns.Count; i++)
                {
                    var value = i < row.Count ? row[i] : string.Empty;
                    var cell = table.Cell()
                        .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);

                    if (striped)
                    {
                        cell = cell.Background(Colors.Grey.Lighten4);
                    }

                    cell.Padding(4).Text(value);
                }

                rowIndex++;
            }
        });
    }

    private static void ComposeFooter(PageDescriptor page, PdfReport report)
    {
        page.Footer().AlignCenter().Text(text =>
        {
            if (!string.IsNullOrWhiteSpace(report.FooterNote))
            {
                text.Span($"{report.FooterNote}   •   ");
            }

            text.Span("Page ");
            text.CurrentPageNumber();
            text.Span(" / ");
            text.TotalPages();
        });
    }

    private static PageSize MapPageSize(PdfPageSize size, bool landscape)
    {
        var pageSize = size switch
        {
            PdfPageSize.A3 => PageSizes.A3,
            PdfPageSize.A5 => PageSizes.A5,
            PdfPageSize.Letter => PageSizes.Letter,
            PdfPageSize.Legal => PageSizes.Legal,
            _ => PageSizes.A4,
        };

        return landscape ? pageSize.Landscape() : pageSize;
    }

    private static Color? ResolveColor(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return null;
        }

        try
        {
            return Color.FromHex(hex);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
