using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;
using System.Globalization;
using Colors = QuestPDF.Helpers.Colors;

namespace ReportHub.Common.Helper.GeneratorHelper
{
    public class PDF_GeneratorHelper : IFormatSpecificGenerator
    {
        public ReportFormat SupportedFormat => ReportFormat.PDF;
        public string ContentType => "application/pdf";

        static PDF_GeneratorHelper()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateAsync(ReportSettings settings, ReportData data)
        {
            return await Task.Run(() => GeneratePdf(settings, data));
        }

        public async Task<Stream> GenerateStreamAsync(ReportSettings settings, ReportData data)
        {
            var bytes = await GenerateAsync(settings, data);
            return new MemoryStream(bytes);
        }

        private byte[] GeneratePdf(ReportSettings settings, ReportData data)
        {
            var culture = new CultureInfo(settings.Culture);
            var isRtl = culture.TextInfo.IsRightToLeft;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(settings.Formatting.FontSize)
                        .FontFamily(Fonts.Arial) // Ensure consistent font
                        .LineHeight(1.2f)); // Better line spacing

                    // Header
                    if (settings.Formatting.IncludeHeader)
                    {
                        page.Header().Element(ComposeHeader);
                    }

                    // Content
                    page.Content().Element(ComposeContent);

                    // Footer
                    if (settings.Formatting.IncludeFooter)
                    {
                        page.Footer().Element(ComposeFooter);
                    }
                });

                void ComposeHeader(IContainer container)
                {
                    container.Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text(settings.Title)
                                .FontSize(20).SemiBold().FontColor(settings.Formatting.PrimaryColor);

                            if (!string.IsNullOrEmpty(settings.Subtitle))
                            {
                                column.Item().Text(settings.Subtitle)
                                    .FontSize(14).FontColor(Colors.Grey.Darken2);
                            }
                        });

                        row.ConstantItem(100).Column(column =>
                        {
                            column.Item().AlignRight().Text($"Generated: {settings.CreatedDate:dd/MM/yyyy}")
                                .FontSize(10);
                            column.Item().AlignRight().Text($"Author: {settings.Author}")
                                .FontSize(10);
                        });
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.Column(column =>
                    {
                        // Add sections
                        var orderedSections = data.Sections.OrderBy(s => s.Order);
                        foreach (var section in orderedSections)
                        {
                            column.Item().PaddingVertical(8).Column(sectionColumn =>
                            {
                                if (!string.IsNullOrEmpty(section.Title))
                                {
                                    sectionColumn.Item().PaddingBottom(8).Text(section.Title)
                                        .FontSize(16).SemiBold().FontColor(settings.Formatting.PrimaryColor);
                                }
                                sectionColumn.Item().Text(section.Content)
                                    .FontSize(11).LineHeight(1.4f);
                            });
                        }

                        // Add tables
                        var orderedTables = data.Tables.OrderBy(t => t.Order);
                        foreach (var table in orderedTables)
                        {
                            column.Item().PaddingVertical(15).Element(container => ComposeTable(container, table));
                        }
                    });
                }

                void ComposeTable(IContainer container, ReportTable table)
                {
                    container.Column(column =>
                    {
                        if (!string.IsNullOrEmpty(table.Title))
                        {
                            column.Item().PaddingBottom(8).Text(table.Title)
                                .FontSize(14).SemiBold().FontColor(settings.Formatting.PrimaryColor);
                        }

                        column.Item().Table(tableBuilder =>
                        {
                            // Define columns with better width distribution
                            tableBuilder.ColumnsDefinition(columns =>
                            {
                                for (int j = 0; j < table.Headers.Count; j++)
                                {
                                    // Adjust column widths based on table type and content
                                    if (table.Headers.Count == 2)
                                    {
                                        if (j == 0)
                                            columns.ConstantColumn(150); // Fixed width for labels/field names
                                        else
                                            columns.RelativeColumn(); // Flexible width for values
                                    }
                                    else if (table.Headers.Count >= 5) // Complex tables like charges
                                    {
                                        // More specific width allocation for complex tables
                                        if (j == 0) columns.ConstantColumn(30);  // No
                                        else if (j == 1) columns.RelativeColumn(3); // Description
                                        else if (j == 2) columns.RelativeColumn(2); // Rate
                                        else if (j == 3) columns.RelativeColumn(2); // Price  
                                        else if (j == 4) columns.RelativeColumn(1); // Quantity
                                        else columns.RelativeColumn(2); // Value
                                    }
                                    else
                                    {
                                        columns.RelativeColumn(); // Default for other tables
                                    }
                                }
                            });

                            // Headers
                            if (table.ShowHeaders)
                            {
                                tableBuilder.Header(header =>
                                {
                                    foreach (var headerText in table.Headers)
                                    {
                                        header.Cell().Element(HeaderCellStyle).Text(headerText).SemiBold();
                                    }
                                });
                            }

                            // Rows
                            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                            {
                                var row = table.Rows[rowIndex];
                                var isAlternate = table.AlternateRowColors && rowIndex % 2 == 1;

                                for (int cellIndex = 0; cellIndex < row.Count && cellIndex < table.Headers.Count; cellIndex++)
                                {
                                    var cellText = row[cellIndex]?.ToString() ?? "";
                                    tableBuilder.Cell()
                                        .Element(cell => isAlternate ? cell.Background(settings.Formatting.SecondaryColor) : cell)
                                        .Element(CellStyle)
                                        .Text(cellText)
                                        .FontSize(10)
                                        .LineHeight(1.2f);
                                }
                            }

                            static IContainer HeaderCellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Background(Colors.Grey.Lighten3)
                                    .PaddingVertical(8)
                                    .PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(6)
                                    .PaddingHorizontal(8)
                                    .AlignLeft()
                                    .AlignMiddle();
                            }
                        });
                    });
                }

                void ComposeFooter(IContainer container)
                {
                    container.Row(row =>
                    {
                        row.RelativeItem().Text("");

                        if (settings.Formatting.IncludePageNumbers)
                        {
                            row.ConstantItem(50).AlignRight().Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                                x.Span(" of ");
                                x.TotalPages();
                            });
                        }
                    });
                }
            }).GeneratePdf();
        }
    }

}


