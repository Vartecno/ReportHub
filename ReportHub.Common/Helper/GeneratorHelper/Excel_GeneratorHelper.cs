using ClosedXML.Excel;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Common.Helper.GeneratorHelper
{
    public class Excel_GeneratorHelper : IFormatSpecificGenerator
    {
        public ReportFormat SupportedFormat => ReportFormat.Excel;
        public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public async Task<byte[]> GenerateAsync(ReportSettings settings, ReportData data)
        {
            return await Task.Run(() => GenerateExcel(settings, data));
        }

        public async Task<Stream> GenerateStreamAsync(ReportSettings settings, ReportData data)
        {
            var bytes = await GenerateAsync(settings, data);
            return new MemoryStream(bytes);
        }

        private byte[] GenerateExcel(ReportSettings settings, ReportData data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");

            int currentRow = 1;

            // Add title and metadata
            worksheet.Cell(currentRow, 1).Value = settings.Title;
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.FromHtml(settings.Formatting.PrimaryColor);
            currentRow += 2;

            if (!string.IsNullOrEmpty(settings.Subtitle))
            {
                worksheet.Cell(currentRow, 1).Value = settings.Subtitle;
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
                worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                currentRow += 2;
            }

            // Metadata
            worksheet.Cell(currentRow, 1).Value = "Generated:";
            worksheet.Cell(currentRow, 2).Value = settings.CreatedDate;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "dd/mm/yyyy hh:mm";
            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Author:";
            worksheet.Cell(currentRow, 2).Value = settings.Author;
            currentRow += 2;

            // Add sections as text
            var orderedSections = data.Sections.OrderBy(s => s.Order);
            foreach (var section in orderedSections)
            {
                if (!string.IsNullOrEmpty(section.Title))
                {
                    worksheet.Cell(currentRow, 1).Value = section.Title;
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                    currentRow++;
                }

                worksheet.Cell(currentRow, 1).Value = section.Content;
                worksheet.Cell(currentRow, 1).Style.Alignment.WrapText = true;
                currentRow += 2;
            }

            // Add tables
            var orderedTables = data.Tables.OrderBy(t => t.Order);
            foreach (var table in orderedTables)
            {
                currentRow = AddTable(worksheet, table, currentRow, settings.Formatting);
                currentRow += 2; // Space between tables
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private int AddTable(IXLWorksheet worksheet, ReportTable table, int startRow, ReportFormattingOptions formatting)
        {
            int currentRow = startRow;

            // Table title
            if (!string.IsNullOrEmpty(table.Title))
            {
                worksheet.Cell(currentRow, 1).Value = table.Title;
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
                currentRow += 2;
            }

            // Headers
            if (table.ShowHeaders && table.Headers.Any())
            {
                for (int i = 0; i < table.Headers.Count; i++)
                {
                    var cell = worksheet.Cell(currentRow, i + 1);
                    cell.Value = table.Headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml(formatting.SecondaryColor);
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
                currentRow++;
            }

            // Data rows
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                var row = table.Rows[rowIndex];

                for (int cellIndex = 0; cellIndex < row.Count && cellIndex < table.Headers.Count; cellIndex++)
                {
                    var cell = worksheet.Cell(currentRow, cellIndex + 1);
                    cell.Value = row[cellIndex]?.ToString() ?? "";
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    if (table.AlternateRowColors && rowIndex % 2 == 1)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                }
                currentRow++;
            }

            return currentRow;
        }
    }
}
