using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportHub.Common.Helper.DataHelpers.IDataHelpers;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Common.Helper.DataHelpers
{
    public class PDFHelper : IPDFHelper
    {
        private readonly ILogger<PDFHelper> _logger;

        public PDFHelper(ILogger<PDFHelper> logger)
        {
            _logger = logger;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public string GetContentType() => "application/pdf";
        public string GetFileExtension() => ".pdf";


        public async Task<byte[]> GenerateInvoiceAsync(InvoiceResponseDTO invoice, CompanyInfo companyInfo)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    if (!string.IsNullOrEmpty(companyInfo.Image))
                                    {
                                        column.Item().Height(60).Placeholder();
                                    }
                                    column.Item().Text(companyInfo.CompanyPrimaryName ?? "Company Name")
                                        .SemiBold().FontSize(20);
                                    column.Item().Text(companyInfo.Address ?? "Company Address");
                                    column.Item().Text($"Tel: {companyInfo.Phone}");
                                    column.Item().Text($"Email: {companyInfo.Email}");
                                });

                                row.ConstantItem(100).Column(column =>
                                {
                                    column.Item().Border(1).Background(Colors.Grey.Lighten3)
                                        .Padding(10).Column(innerColumn =>
                                        {
                                            innerColumn.Item().Text("INVOICE").Bold().FontSize(16);
                                            innerColumn.Item().Text($"Date: {invoice.InvoiceTo?.Date:yyyy-MM-dd}");
                                            innerColumn.Item().Text($"Invoice #: {invoice.InvoiceTo?.InvoiceNumber}");
                                            innerColumn.Item().Text($"Reference: {invoice.InvoiceTo?.NubaReference}");
                                        });
                                });
                            });

                        page.Content()
                            .Column(column =>
                            {
                                column.Item().Text("Invoice To:").SemiBold().FontSize(14);
                                column.Item().PaddingBottom(10).Text(invoice.InvoiceTo?.CompanyName ?? "Customer Name");

                                if (invoice.ShipmentDetails != null)
                                {
                                    column.Item().PaddingTop(15).Text("Shipment Details").SemiBold().FontSize(14);
                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3);
                                            columns.RelativeColumn(3);
                                            columns.RelativeColumn(3);
                                            columns.RelativeColumn(3);
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Shipper").SemiBold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Consignee").SemiBold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Service").SemiBold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Origin/Destination").SemiBold();
                                        });

                                        table.Cell().Border(1).Padding(5).Text(invoice.ShipmentDetails.Shipper);
                                        table.Cell().Border(1).Padding(5).Text(invoice.ShipmentDetails.Consignee);
                                        table.Cell().Border(1).Padding(5).Text(invoice.ShipmentDetails.Service);
                                        table.Cell().Border(1).Padding(5).Text($"{invoice.ShipmentDetails.Origin} / {invoice.ShipmentDetails.Destination}");

                                        table.Cell().Border(1).Padding(5).Text($"HBL: {invoice.ShipmentDetails.HBL}");
                                        table.Cell().Border(1).Padding(5).Text($"MBL: {invoice.ShipmentDetails.MBL}");
                                        table.Cell().Border(1).Padding(5).Text($"Weight: {invoice.ShipmentDetails.Weight:F2} KGS");
                                        table.Cell().Border(1).Padding(5).Text($"Volume: {invoice.ShipmentDetails.Volume}");
                                    });
                                }

                                if (invoice.Charges?.Any() == true)
                                {
                                    column.Item().PaddingTop(20).Text("Charges").SemiBold().FontSize(14);
                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(4);
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(2);
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("No").SemiBold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Description").SemiBold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Rate Per").SemiBold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Unit Price").SemiBold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Quantity").SemiBold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Value").SemiBold();
                                        });

                                        for (int i = 0; i < invoice.Charges.Count; i++)
                                        {
                                            var charge = invoice.Charges[i];
                                            table.Cell().Border(1).Padding(5).Text((i + 1).ToString());
                                            table.Cell().Border(1).Padding(5).Text(charge.Description);
                                            table.Cell().Border(1).Padding(5).Text(charge.RatePER);
                                            table.Cell().Border(1).Padding(5).Text($"{charge.CurrencyName} {charge.UnitPrice:F2}");
                                            table.Cell().Border(1).Padding(5).Text(charge.Quantity.ToString("F4"));
                                            table.Cell().Border(1).Padding(5).Text($"JOD {charge.Value:F2}");
                                        }

                                        // Total row
                                        table.Cell().ColumnSpan(5).Border(1).Background(Colors.Grey.Lighten4)
                                            .Padding(5).AlignRight().Text("Total:").SemiBold();
                                        table.Cell().Border(1).Background(Colors.Grey.Lighten4)
                                            .Padding(5).Text($"JOD {invoice.ChargesTotal?.TotalDue:F2}").SemiBold();
                                    });
                                }

                                // Bank Details
                                if (invoice.BankWithDetails?.Any() == true)
                                {
                                    column.Item().PaddingTop(20).Text("Payment Details").SemiBold().FontSize(14);
                                    foreach (var bank in invoice.BankWithDetails)
                                    {
                                        column.Item().PaddingTop(5).Text($"Bank: {bank.BankPrimaryName}");
                                        column.Item().Text($"IBAN: {bank.IBAN}");
                                        column.Item().Text($"SWIFT: {bank.SWIFT}");
                                        column.Item().Text($"Account: {bank.AccountBankNo}");
                                    }
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                    });
                });

                return document.GeneratePdf();
            });
        }

      
    }
}
