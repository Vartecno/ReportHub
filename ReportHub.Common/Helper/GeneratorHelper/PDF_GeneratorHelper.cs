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
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                    page.Content().Element(ComposeInvoice);
                });

                void ComposeInvoice(IContainer container)
                {
                    container.Column(column =>
                    {
                        // Company Address Header
                        column.Item().PaddingBottom(10).Text("Amman-Jordan, Mecca Street, Rashed Al-Neimat Building No.9, 3rd Floor, Office No.301")
                            .FontSize(10);

                        // Company Name
                        column.Item().PaddingBottom(15).Text("Nuba logistics")
                            .FontSize(14).SemiBold();

                        // Invoice Title (centered)
                        column.Item().PaddingBottom(15).AlignCenter().Text("Invoice")
                            .FontSize(18).SemiBold();

                        // Date and Invoice Details
                        column.Item().PaddingBottom(15).Column(detailsColumn =>
                        {
                            detailsColumn.Item().Text($"Date: {settings.CreatedDate:d-M-yyyy}")
                                .FontSize(11);
                            detailsColumn.Item().Text($"Invoice Number: {(data.Variables.ContainsKey("invoiceNumber") ? data.Variables["invoiceNumber"] : "")}")
                                .FontSize(11);
                            detailsColumn.Item().Text($"Nuba Reference: {(data.Variables.ContainsKey("nubaReference") ? data.Variables["nubaReference"] : "")}")
                                .FontSize(11);
                        });

                        // Shipment Details
                        var shipmentTable = data.Tables.FirstOrDefault(t => t.Title == "Shipment Details");
                        if (shipmentTable != null)
                        {
                            column.Item().PaddingBottom(15).Column(shipmentColumn =>
                            {
                                shipmentColumn.Item().PaddingBottom(5).Text("Shipment Details")
                                    .FontSize(12).SemiBold();

                                // Create a compact table layout matching the reference
                                shipmentColumn.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(3);
                                    });

                                    foreach (var row in shipmentTable.Rows)
                                    {
                                        if (row.Count >= 2)
                                        {
                                            table.Cell().Text(row[0]).FontSize(10).SemiBold();
                                            table.Cell().Text(row[1]).FontSize(10);
                                        }
                                    }
                                });
                            });
                        }

                        // Charges
                        var chargesTable = data.Tables.FirstOrDefault(t => t.Title == "Charges");
                        if (chargesTable != null)
                        {
                            column.Item().PaddingBottom(15).Column(chargesColumn =>
                            {
                                chargesColumn.Item().PaddingBottom(5).Text("Charges")
                                    .FontSize(12).SemiBold();

                                chargesColumn.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(30);
                                        columns.RelativeColumn();
                                        columns.ConstantColumn(100);
                                        columns.ConstantColumn(100);
                                        columns.ConstantColumn(80);
                                        columns.ConstantColumn(100);
                                    });

                                    // Header
                                    table.Cell().Text("No").FontSize(10).SemiBold();
                                    table.Cell().Text("Description").FontSize(10).SemiBold();
                                    table.Cell().Text("Rate Per Unit").FontSize(10).SemiBold();
                                    table.Cell().Text("Price").FontSize(10).SemiBold();
                                    table.Cell().Text("Quantity").FontSize(10).SemiBold();
                                    table.Cell().Text("Value").FontSize(10).SemiBold();

                                    // Rows
                                    foreach (var row in chargesTable.Rows)
                                    {
                                        for (int i = 0; i < 6 && i < row.Count; i++)
                                        {
                                            table.Cell().Text(row[i]).FontSize(10);
                                        }
                                    }
                                });
                            });
                        }

                        // Total Amount
                        var totalTable = data.Tables.FirstOrDefault(t => t.Title == "Total Amount");
                        if (totalTable != null && totalTable.Rows.Count > 0)
                        {
                            column.Item().PaddingBottom(15).AlignRight()
                                .Text($"Total Invoice Amount: {totalTable.Rows[0][1]}")
                                .FontSize(12).SemiBold();
                        }

                        // Scan Code Note
                        column.Item().PaddingBottom(10).Text("Scan Code is only available on Sanad-Jo")
                            .FontSize(9);

                        // Payment Details
                        var paymentTable = data.Tables.FirstOrDefault(t => t.Title == "Payment Details");
                        if (paymentTable != null)
                        {
                            column.Item().Column(paymentColumn =>
                            {
                                paymentColumn.Item().PaddingBottom(5).Text("Payment Details")
                                    .FontSize(12).SemiBold();

                                paymentColumn.Item().Text("Beneficiary Name: Nuba Logistics")
                                    .FontSize(10).SemiBold();

                                // Group accounts by currency
                                var jodAccount = paymentTable.Rows.FirstOrDefault(r => r[0] == "JOD Account");
                                var jodAccountNo = paymentTable.Rows.FirstOrDefault(r => r[0] == "JOD Account No");
                                if (jodAccount != null && jodAccountNo != null)
                                {
                                    paymentColumn.Item().Text($"Account Currency: JOD Account")
                                        .FontSize(10);
                                    paymentColumn.Item().Text($"IBAN: {jodAccount[1]}")
                                        .FontSize(10);
                                    paymentColumn.Item().Text($"Account No: {jodAccountNo[1]}")
                                        .FontSize(10);
                                    paymentColumn.Item().Text("SWIFT: ARABJOAX100")
                                        .FontSize(10);
                                }

                                var usdAccount = paymentTable.Rows.FirstOrDefault(r => r[0] == "USD Account");
                                var usdAccountNo = paymentTable.Rows.FirstOrDefault(r => r[0] == "USD Account No");
                                if (usdAccount != null && usdAccountNo != null)
                                {
                                    paymentColumn.Item().Text($"Account Currency: USD Account")
                                        .FontSize(10);
                                    paymentColumn.Item().Text($"IBAN: {usdAccount[1]}")
                                        .FontSize(10);
                                    paymentColumn.Item().Text($"Account No: {usdAccountNo[1]}")
                                        .FontSize(10);
                                    paymentColumn.Item().Text("SWIFT: ARABJOAX100")
                                        .FontSize(10);
                                }

                                var eurAccount = paymentTable.Rows.FirstOrDefault(r => r[0] == "EUR Account");
                                var eurAccountNo = paymentTable.Rows.FirstOrDefault(r => r[0] == "EUR Account No");
                                if (eurAccount != null && eurAccountNo != null)
                                {
                                    paymentColumn.Item().Text($"Account Currency: EUR Account")
                                        .FontSize(10);
                                    paymentColumn.Item().Text($"IBAN: {eurAccount[1]}")
                                        .FontSize(10);
                                    paymentColumn.Item().Text($"Account No: {eurAccountNo[1]}")
                                        .FontSize(10);
                                    paymentColumn.Item().Text("SWIFT: ARABJOAX100")
                                        .FontSize(10);
                                }

                                // ClicQ Service
                                var clicqService = paymentTable.Rows.FirstOrDefault(r => r[0] == "ClicQ Service");
                                if (clicqService != null)
                                {
                                    paymentColumn.Item().Text($"ClicQ Service: {clicqService[1]}")
                                        .FontSize(10);
                                }
                            });
                        }
                    });
                }
            }).GeneratePdf();
        }
    }

}


