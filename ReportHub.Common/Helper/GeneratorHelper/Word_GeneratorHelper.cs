using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;

namespace ReportHub.Common.Helper.GeneratorHelper
{
    public class Word_GeneratorHelper : IFormatSpecificGenerator
    {
        public ReportFormat SupportedFormat => ReportFormat.Word;
        public string ContentType => "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        public async Task<byte[]> GenerateAsync(ReportSettings settings, ReportData data)
        {
            return await Task.Run(() => GenerateWord(settings, data));
        }

        public async Task<Stream> GenerateStreamAsync(ReportSettings settings, ReportData data)
        {
            var bytes = await GenerateAsync(settings, data);
            return new MemoryStream(bytes);
        }

        private byte[] GenerateWord(ReportSettings settings, ReportData data)
        {
            using var memoryStream = new MemoryStream();
            using var document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);

            // Create main document part
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // Add title
            AddTitle(body, settings.Title, settings.Formatting);

            if (!string.IsNullOrEmpty(settings.Subtitle))
            {
                AddSubtitle(body, settings.Subtitle, settings.Formatting);
            }

            // Add metadata paragraph
            AddMetadata(body, settings);

            // Add sections
            var orderedSections = data.Sections.OrderBy(s => s.Order);
            foreach (var section in orderedSections)
            {
                AddSection(body, section, settings.Formatting);
            }

            // Add tables
            var orderedTables = data.Tables.OrderBy(t => t.Order);
            foreach (var table in orderedTables)
            {
                AddTable(body, table, settings.Formatting);
            }

            document.Save();
            return memoryStream.ToArray();
        }

        private void AddTitle(Body body, string title, ReportFormattingOptions formatting)
        {
            var titleParagraph = new Paragraph();
            var titleRun = new Run();
            var titleRunProperties = new RunProperties();

            titleRunProperties.Append(new Bold());
            titleRunProperties.Append(new FontSize { Val = "28" });
            titleRunProperties.Append(new Color { Val = formatting.PrimaryColor.Replace("#", "") });

            titleRun.Append(titleRunProperties);
            titleRun.Append(new Text(title));
            titleParagraph.Append(titleRun);

            body.Append(titleParagraph);
        }

        private void AddSubtitle(Body body, string subtitle, ReportFormattingOptions formatting)
        {
            var subtitleParagraph = new Paragraph();
            var subtitleRun = new Run();
            var subtitleRunProperties = new RunProperties();

            subtitleRunProperties.Append(new FontSize { Val = "18" });
            subtitleRunProperties.Append(new Color { Val = "666666" });

            subtitleRun.Append(subtitleRunProperties);
            subtitleRun.Append(new Text(subtitle));
            subtitleParagraph.Append(subtitleRun);

            body.Append(subtitleParagraph);
        }

        private void AddMetadata(Body body, ReportSettings settings)
        {
            var metadataParagraph = new Paragraph();
            var metadataRun = new Run();
            var metadataRunProperties = new RunProperties();

            metadataRunProperties.Append(new FontSize { Val = "16" });
            metadataRunProperties.Append(new Italic());

            var metadataText = $"Generated on {settings.CreatedDate:yyyy-MM-dd HH:mm} by {settings.Author}";

            metadataRun.Append(metadataRunProperties);
            metadataRun.Append(new Text(metadataText));
            metadataParagraph.Append(metadataRun);

            body.Append(metadataParagraph);
            body.Append(new Paragraph()); // Empty line
        }

        private void AddSection(Body body, ReportSection section, ReportFormattingOptions formatting)
        {
            if (!string.IsNullOrEmpty(section.Title))
            {
                var titleParagraph = new Paragraph();
                var titleRun = new Run();
                var titleRunProperties = new RunProperties();

                titleRunProperties.Append(new Bold());
                titleRunProperties.Append(new FontSize { Val = "20" });

                titleRun.Append(titleRunProperties);
                titleRun.Append(new Text(section.Title));
                titleParagraph.Append(titleRun);

                body.Append(titleParagraph);
            }

            var contentParagraph = new Paragraph();
            var contentRun = new Run(new Text(section.Content));
            contentParagraph.Append(contentRun);

            body.Append(contentParagraph);
            body.Append(new Paragraph()); // Empty line
        }

        private void AddTable(Body body, ReportTable reportTable, ReportFormattingOptions formatting)
        {
            if (!string.IsNullOrEmpty(reportTable.Title))
            {
                var titleParagraph = new Paragraph();
                var titleRun = new Run();
                var titleRunProperties = new RunProperties();

                titleRunProperties.Append(new Bold());
                titleRunProperties.Append(new FontSize { Val = "18" });

                titleRun.Append(titleRunProperties);
                titleRun.Append(new Text(reportTable.Title));
                titleParagraph.Append(titleRun);

                body.Append(titleParagraph);
            }

            var table = new Table();

            // Table properties
            var tableProperties = new TableProperties();
            var tableBorders = new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }
            );
            tableProperties.Append(tableBorders);
            table.AppendChild(tableProperties);

            // Headers
            if (reportTable.ShowHeaders && reportTable.Headers.Any())
            {
                var headerRow = new TableRow();
                foreach (var header in reportTable.Headers)
                {
                    var cell = new TableCell();
                    var cellProperties = new TableCellProperties();
                    cellProperties.Append(new Shading { Val = ShadingPatternValues.Clear, Fill = formatting.SecondaryColor.Replace("#", "") });
                    cell.Append(cellProperties);

                    var paragraph = new Paragraph();
                    var run = new Run();
                    var runProperties = new RunProperties();
                    runProperties.Append(new Bold());
                    run.Append(runProperties);
                    run.Append(new Text(header));
                    paragraph.Append(run);
                    cell.Append(paragraph);
                    headerRow.Append(cell);
                }
                table.Append(headerRow);
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < reportTable.Rows.Count; rowIndex++)
            {
                var row = reportTable.Rows[rowIndex];
                var tableRow = new TableRow();

                for (int cellIndex = 0; cellIndex < row.Count && cellIndex < reportTable.Headers.Count; cellIndex++)
                {
                    var cell = new TableCell();

                    if (reportTable.AlternateRowColors && rowIndex % 2 == 1)
                    {
                        var cellProperties = new TableCellProperties();
                        cellProperties.Append(new Shading { Val = ShadingPatternValues.Clear, Fill = "F0F0F0" });
                        cell.Append(cellProperties);
                    }

                    var paragraph = new Paragraph();
                    var run = new Run(new Text(row[cellIndex]?.ToString() ?? ""));
                    paragraph.Append(run);
                    cell.Append(paragraph);
                    tableRow.Append(cell);
                }
                table.Append(tableRow);
            }

            body.Append(table);
            body.Append(new Paragraph()); // Empty line
        }
    }
   
}
