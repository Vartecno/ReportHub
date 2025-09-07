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
                    page.DefaultTextStyle(x => x.FontSize(settings.Formatting.FontSize));

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
                            column.Item().PaddingVertical(5).Column(sectionColumn =>
                            {
                                if (!string.IsNullOrEmpty(section.Title))
                                {
                                    sectionColumn.Item().PaddingBottom(5).Text(section.Title)
                                    .FontSize(16).SemiBold();
                               
                                }
                                sectionColumn.Item().Text(section.Content);
                            });
                        }

                        // Add tables
                        var orderedTables = data.Tables.OrderBy(t => t.Order);
                        foreach (var table in orderedTables)
                        {
                            column.Item().PaddingVertical(10).Element(container => ComposeTable(container, table));
                        }
                    });
                }

                void ComposeTable(IContainer container, ReportTable table)
                {
                    container.Column(column =>
                    {
                        if (!string.IsNullOrEmpty(table.Title))
                        {
                            column.Item().PaddingBottom(5).Text(table.Title)
                                .FontSize(14).SemiBold();
                        }

                        column.Item().Table(tableBuilder =>
                        {
                            // Define columns only once
                            tableBuilder.ColumnsDefinition(columns =>
                            {
                                for (int j = 0; j < table.Headers.Count; j++)
                                {
                                    columns.RelativeColumn();
                                }
                            });

                            // Headers
                            if (table.ShowHeaders)
                            {
                                tableBuilder.Header(header =>
                                {
                                    foreach (var headerText in table.Headers)
                                    {
                                        header.Cell().Element(CellStyle).Text(headerText).SemiBold();
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
                                    tableBuilder.Cell()
                                        .Element(cell => isAlternate ? cell.Background(settings.Formatting.SecondaryColor) : cell)
                                        .Element(CellStyle)
                                        .Text(row[cellIndex]?.ToString() ?? "");
                                }
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
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


