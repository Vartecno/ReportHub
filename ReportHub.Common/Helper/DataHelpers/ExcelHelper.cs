using Microsoft.Extensions.Logging;
using ReportHub.Common.Helper.DataHelpers.IDataHelpers;
using ReportHub.Objects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Common.Helper.DataHelpers
{
    public class ExcelHelper: IExcelHelper
    {
        private readonly ILogger<ExcelHelper> _logger;

        public ExcelHelper(ILogger<ExcelHelper> logger)
        {
            _logger = logger;
        }

        public string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public string GetFileExtension() => ".xlsx";


        public async Task<byte[]> GenerateProfitReportAsync(List<OrderProfitResultDTO> profits, DateTime fromDate, DateTime toDate)
        {
            return await Task.Run(() =>
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Profit Report");

                var headerRow = 1;
                worksheet.Cell(headerRow, 1).Value = $"Profit Report - ({fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd})";
                worksheet.Cell(headerRow, 1).Style.Font.FontSize = 16;
                worksheet.Cell(headerRow, 1).Style.Font.Bold = true;
                worksheet.Range(headerRow, 1, headerRow, 10).Merge();

                var row = 3;
                worksheet.Cell(row, 1).Value = "Order Lead Code";
                worksheet.Cell(row, 2).Value = "Invoice No.";
                worksheet.Cell(row, 3).Value = "Total Sales Price";
                worksheet.Cell(row, 4).Value = "Total Cost";
                worksheet.Cell(row, 5).Value = "Description";
                worksheet.Cell(row, 6).Value = "Weight";
                worksheet.Cell(row, 7).Value = "Volume";
                worksheet.Cell(row, 8).Value = "Profit";
                worksheet.Cell(row, 9).Value = "Customer";
                worksheet.Cell(row, 10).Value = "Currency";

                var headerRange = worksheet.Range(row, 1, row, 10);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                row++;
                foreach (var item in profits)
                {
                    worksheet.Cell(row, 1).Value = item.OrderLeadCode;
                    worksheet.Cell(row, 2).Value = item.InvoiceID;
                    worksheet.Cell(row, 3).Value = item.TotalSalesPrice;
                    worksheet.Cell(row, 4).Value = item.TotalCost;
                    worksheet.Cell(row, 5).Value = item.Description;
                    worksheet.Cell(row, 6).Value = item.Weight;
                    worksheet.Cell(row, 7).Value = item.Volume;
                    worksheet.Cell(row, 8).Value = item.Profit;
                    worksheet.Cell(row, 9).Value = item.CustomerName;
                    worksheet.Cell(row, 10).Value = item.CurrencyCode;

                    worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";

                    row++;
                }

                if (profits.Any())
                {
                    row++;
                    worksheet.Cell(row, 2).Value = "TOTALS:";
                    worksheet.Cell(row, 2).Style.Font.Bold = true;

                    worksheet.Cell(row, 3).FormulaA1 = $"=SUM(C4:C{row - 2})";
                    worksheet.Cell(row, 4).FormulaA1 = $"=SUM(D4:D{row - 2})";
                    worksheet.Cell(row, 8).FormulaA1 = $"=SUM(H4:H{row - 2})";

                    var totalRange = worksheet.Range(row, 2, row, 10);
                    totalRange.Style.Font.Bold = true;
                    totalRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            });
        }

        public async Task<byte[]> GenerateOrderDetailsReportAsync(List<OrderLeadDetailsDTO> orderDetails)
        {
            return await Task.Run(() =>
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Order Details");

                var headerRow = 1;
                worksheet.Cell(headerRow, 1).Value = "Order Details Report";
                worksheet.Cell(headerRow, 1).Style.Font.FontSize = 16;
                worksheet.Cell(headerRow, 1).Style.Font.Bold = true;
                worksheet.Range(headerRow, 1, headerRow, 8).Merge();

                var row = 3;
                worksheet.Cell(row, 1).Value = "Code";
                worksheet.Cell(row, 2).Value = "Description";
                worksheet.Cell(row, 3).Value = "Quantity";
                worksheet.Cell(row, 4).Value = "Sales Price";
                worksheet.Cell(row, 5).Value = "Cost";
                worksheet.Cell(row, 6).Value = "Profit";
                worksheet.Cell(row, 7).Value = "Customer";
                worksheet.Cell(row, 8).Value = "Currency";

                var headerRange = worksheet.Range(row, 1, row, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

                row++;
                foreach (var item in orderDetails)
                {
                    worksheet.Cell(row, 1).Value = item.Code;
                    worksheet.Cell(row, 2).Value = item.Description;
                    worksheet.Cell(row, 3).Value = item.Quantity;
                    worksheet.Cell(row, 4).Value = item.SalesPrice;
                    worksheet.Cell(row, 5).Value = item.Cost;
                    worksheet.Cell(row, 6).Value = item.Profit;
                    worksheet.Cell(row, 7).Value = item.CustomerName;
                    worksheet.Cell(row, 8).Value = item.CurrencyCode;
                    
                    worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            });
        }

        public async Task<byte[]> GenerateCustomReportAsync(Dictionary<string, List<object>> sheets)
        {
            return await Task.Run(() =>
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook();

                foreach (var sheet in sheets)
                {
                    var worksheet = workbook.Worksheets.Add(sheet.Key);
                    var data = sheet.Value;

                    if (data.Any())
                    {
                        var firstItem = data.First();
                        var properties = firstItem.GetType().GetProperties();

                        for (int col = 0; col < properties.Length; col++)
                        {
                            worksheet.Cell(1, col + 1).Value = properties[col].Name;
                            worksheet.Cell(1, col + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, col + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                        }

                        for (int row = 0; row < data.Count; row++)
                        {
                            for (int col = 0; col < properties.Length; col++)
                            {
                                var value = properties[col].GetValue(data[row]);
                                worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? "";
                            }
                        }

                        worksheet.Columns().AdjustToContents();
                    }
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            });
        }

        private async Task<byte[]> GenerateGenericExcelReportAsync(ReportDataDTO data, string reportName, Dictionary<string, object> parameters)
        {
            return await Task.Run(() =>
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add(reportName);

                worksheet.Cell(1, 1).Value = $"{reportName} Report";
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Cell(1, 1).Style.Font.Bold = true;

                worksheet.Cell(3, 1).Value = "This is a generic Excel report template.";
                worksheet.Cell(4, 1).Value = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            });
        }

        private DateTime GetDateParameter(Dictionary<string, object> parameters, string key)
        {
            if (parameters.ContainsKey(key) && DateTime.TryParse(parameters[key]?.ToString(), out var date))
                return date;
            return DateTime.Now;
        }
    }
}

