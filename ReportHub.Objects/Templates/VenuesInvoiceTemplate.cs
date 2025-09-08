using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.Interfaces;
using Colors = QuestPDF.Helpers.Colors;

namespace ReportHub.Objects.Templates
{
    /// <summary>
    /// Template for Venues Invoice reports (logistics/shipping invoices)
    /// Matches the pixel-perfect layout of the original sample
    /// </summary>
    public class VenuesInvoiceTemplate : BaseReportTemplate
    {
        public override string TemplateId => "venues_invoice";
        public override string DisplayName => "Venues Invoice";
        public override string Description => "Professional invoice template for logistics and shipping services";
        public override Type[] SupportedDataTypes => new[] { typeof(VenuesInvoiceDataDTO) };

        public override void GenerateContent(IContainer container, TemplateReportRequestDTO request)
        {
            var invoiceData = ParseTemplateData<VenuesInvoiceDataDTO>(request.Data);
            if (invoiceData == null)
                throw new ArgumentException("Invalid data format for VenuesInvoiceTemplate");

            container.Column(column =>
                {
                    // Company Header with Logo
                    column.Item().PaddingBottom(20).Element(headerContainer => 
                        RenderInvoiceHeader(headerContainer, request.Branding, invoiceData.Header));

                    // Invoice Title (centered)
                    column.Item().PaddingBottom(15).AlignCenter().Text("Invoice")
                        .FontSize(18).SemiBold().FontColor(request.Branding.Colors.Primary);

                    // Invoice Details
                    column.Item().PaddingBottom(15).Element(detailsContainer => 
                        RenderInvoiceDetails(detailsContainer, invoiceData.Header, request.Branding.Typography));

                    // Shipment Details Section
                    if (invoiceData.Shipment != null)
                    {
                        column.Item().PaddingBottom(15).Element(shipmentContainer => 
                            RenderShipmentDetails(shipmentContainer, invoiceData.Shipment, request.Branding));
                    }

                    // Charges Table
                    if (invoiceData.Charges?.Any() == true)
                    {
                        column.Item().PaddingBottom(15).Element(chargesContainer => 
                            RenderChargesTable(chargesContainer, invoiceData.Charges, request.Branding));
                    }

                    // Total Amount
                    if (invoiceData.Total != null)
                    {
                        column.Item().PaddingBottom(15).Element(totalContainer => 
                            RenderTotalAmount(totalContainer, invoiceData.Total, request.Branding));
                    }

                    // Scan Code Note
                    column.Item().PaddingBottom(10).Text("Scan Code is only available on Sanad-Jo")
                        .FontSize(request.Branding.Typography.SmallSize)
                        .FontColor(Colors.Grey.Medium);

                    // Payment Details
                    if (invoiceData.BankAccounts?.Any() == true)
                    {
                        column.Item().Element(paymentContainer => 
                            RenderPaymentDetails(paymentContainer, invoiceData.BankAccounts, request.Branding));
                    }
                });
        }
        }

        private void RenderInvoiceHeader(IContainer container, ReportBrandingDTO branding, InvoiceHeaderDTO header)
        {
            container.Row(row =>
            {
                // Logo section
                if (!string.IsNullOrWhiteSpace(branding.Logo))
                {
                    row.ConstantItem(150).Element(logoContainer => RenderLogo(logoContainer, branding.Logo));
                    row.RelativeItem().PaddingLeft(20);
                }
                else
                {
                    row.RelativeItem();
                }

                // Company info section
                row.RelativeItem().Column(infoColumn =>
                {
                    // Company address first (matching original layout)
                    if (!string.IsNullOrWhiteSpace(branding.Company.Address))
                    {
                        infoColumn.Item().Text(branding.Company.Address)
                            .FontSize(branding.Typography.SmallSize);
                    }

                    // Company name
                    infoColumn.Item().PaddingTop(5).Text(branding.Company.Name)
                        .FontSize(14).SemiBold();
                });
            });
        }

        private void RenderInvoiceDetails(IContainer container, InvoiceHeaderDTO header, TypographyDTO typography)
        {
            container.Column(column =>
            {
                column.Item().Text($"Date: {FormatDate(header.Date)}")
                    .FontSize(typography.BodySize);
                
                if (!string.IsNullOrWhiteSpace(header.InvoiceNumber))
                {
                    column.Item().Text($"Invoice Number: {header.InvoiceNumber}")
                        .FontSize(typography.BodySize);
                }
                
                if (!string.IsNullOrWhiteSpace(header.Reference))
                {
                    column.Item().Text($"Nuba Reference: {header.Reference}")
                        .FontSize(typography.BodySize);
                }
            });
        }

        private void RenderShipmentDetails(IContainer container, ShipmentDetailsDTO shipment, ReportBrandingDTO branding)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(5).Text("Shipment Details")
                    .FontSize(12).SemiBold();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(3);
                    });

                    // Add shipment details rows
                    AddShipmentRow(table, "Service", shipment.Service);
                    AddShipmentRow(table, "Origin", shipment.Origin);
                    AddShipmentRow(table, "Destination", shipment.Destination);
                    AddShipmentRow(table, "AWB", shipment.AWB);
                    AddShipmentRow(table, "MBL", shipment.MBL);
                    AddShipmentRow(table, "HBL", shipment.HBL);
                    AddShipmentRow(table, "Weight", $"{shipment.Weight} kg");
                    AddShipmentRow(table, "Volume", shipment.Volume);
                });
            });
        }

        private void AddShipmentRow(TableDescriptor table, string label, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                table.Cell().Text(label).FontSize(10).SemiBold();
                table.Cell().Text(value).FontSize(10);
            }
        }

        private void RenderChargesTable(IContainer container, List<ChargeItemDTO> charges, ReportBrandingDTO branding)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(5).Text("Charges")
                    .FontSize(12).SemiBold();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // No
                        columns.RelativeColumn();     // Description
                        columns.ConstantColumn(100);  // Rate Per Unit
                        columns.ConstantColumn(100);  // Price
                        columns.ConstantColumn(80);   // Quantity
                        columns.ConstantColumn(100);  // Value
                    });

                    // Header row
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("No").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Description").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Rate Per Unit").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Price").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Quantity").FontSize(10).SemiBold().FontColor(Colors.White);
                    table.Cell().Background(branding.Colors.Primary).Padding(5)
                        .Text("Value").FontSize(10).SemiBold().FontColor(Colors.White);

                    // Data rows
                    for (int i = 0; i < charges.Count; i++)
                    {
                        var charge = charges[i];
                        var isEvenRow = i % 2 == 0;
                        var bgColor = isEvenRow ? Colors.White : Colors.Grey.Lighten4;

                        table.Cell().Background(bgColor).Padding(5)
                            .Text((i + 1).ToString()).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(charge.Description).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(FormatCurrency(charge.RatePerUnit, charge.Currency)).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(FormatCurrency(charge.Price, charge.Currency)).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(charge.Quantity.ToString("N2")).FontSize(10);
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(FormatCurrency(charge.Value, charge.Currency)).FontSize(10);
                    }
                });
            });
        }

        private void RenderTotalAmount(IContainer container, InvoiceTotalDTO total, ReportBrandingDTO branding)
        {
            container.AlignRight().Text($"Total Invoice Amount: {FormatCurrency(total.Total, total.Currency)}")
                .FontSize(12).SemiBold().FontColor(branding.Colors.Primary);
        }

        private void RenderPaymentDetails(IContainer container, List<BankAccountDTO> bankAccounts, ReportBrandingDTO branding)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(5).Text("Payment Details")
                    .FontSize(12).SemiBold();

                column.Item().Text($"Beneficiary Name: {branding.Company.Name}")
                    .FontSize(10).SemiBold();

                // Group accounts by currency
                foreach (var account in bankAccounts)
                {
                    column.Item().PaddingTop(5).Column(accountColumn =>
                    {
                        accountColumn.Item().Text($"Account Currency: {account.Currency} Account")
                            .FontSize(10);
                        accountColumn.Item().Text($"IBAN: {account.IBAN}")
                            .FontSize(10);
                        accountColumn.Item().Text($"Account No: {account.AccountNumber}")
                            .FontSize(10);
                        accountColumn.Item().Text($"SWIFT: {account.SWIFT}")
                            .FontSize(10);
                    });
                }
            });
        }

        public override TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request)
        {
            var result = base.ValidateRequest(request);
            
            if (result.IsValid)
            {
                var invoiceData = ParseTemplateData<VenuesInvoiceDataDTO>(request.Data);
                if (invoiceData == null)
                {
                    result.Errors.Add("Data must be of type VenuesInvoiceDataDTO");
                }
                else
                {
                    if (invoiceData.Header == null)
                        result.Errors.Add("Invoice header is required");
                    if (invoiceData.Charges == null || !invoiceData.Charges.Any())
                        result.Warnings.Add("No charges found in invoice");
                }
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }

        public override object GetSampleData()
        {
            return new VenuesInvoiceDataDTO
            {
                Header = new InvoiceHeaderDTO
                {
                    InvoiceNumber = "INV-2025-001",
                    Date = DateTime.Now,
                    Reference = "NUBA-REF-12345",
                    CustomerName = "Sample Customer",
                    CustomerAddress = "123 Customer Street, City, Country"
                },
                Shipment = new ShipmentDetailsDTO
                {
                    Service = "FCL Sea Freight",
                    Origin = "Shanghai",
                    Destination = "Aqaba",
                    AWB = "AWB123456",
                    MBL = "MBL789012",
                    HBL = "HBL345678",
                    Weight = 1500.50m,
                    Volume = "25 CBM"
                },
                Charges = new List<ChargeItemDTO>
                {
                    new ChargeItemDTO
                    {
                        Sequence = 1,
                        Description = "Sea Freight",
                        RatePerUnit = 100.00m,
                        Quantity = 1,
                        Price = 100.00m,
                        Value = 100.00m,
                        Currency = "JOD"
                    },
                    new ChargeItemDTO
                    {
                        Sequence = 2,
                        Description = "Documentation Fee",
                        RatePerUnit = 25.00m,
                        Quantity = 1,
                        Price = 25.00m,
                        Value = 25.00m,
                        Currency = "JOD"
                    }
                },
                Total = new InvoiceTotalDTO
                {
                    Subtotal = 125.00m,
                    Tax = 0m,
                    Total = 125.00m,
                    Currency = "JOD"
                },
                BankAccounts = new List<BankAccountDTO>
                {
                    new BankAccountDTO
                    {
                        BankName = "Arab Bank",
                        AccountName = "Nuba Logistics",
                        AccountNumber = "123456789",
                        IBAN = "JO94ARAB1234567890123456",
                        SWIFT = "ARABJOAX100",
                        Currency = "JOD"
                    }
                }
            };
        }

        public override TemplateConfigSchema GetConfigSchema()
        {
            return new TemplateConfigSchema
            {
                TemplateId = TemplateId,
                RequiredFields = new Dictionary<string, FieldSchema>
                {
                    { "Header", new FieldSchema { Name = "Header", Type = "InvoiceHeaderDTO", Description = "Invoice header information", IsRequired = true } },
                    { "Charges", new FieldSchema { Name = "Charges", Type = "List<ChargeItemDTO>", Description = "List of invoice charges", IsRequired = true } }
                },
                OptionalFields = new Dictionary<string, FieldSchema>
                {
                    { "Shipment", new FieldSchema { Name = "Shipment", Type = "ShipmentDetailsDTO", Description = "Shipment details", IsRequired = false } },
                    { "Total", new FieldSchema { Name = "Total", Type = "InvoiceTotalDTO", Description = "Invoice totals", IsRequired = false } },
                    { "BankAccounts", new FieldSchema { Name = "BankAccounts", Type = "List<BankAccountDTO>", Description = "Payment bank accounts", IsRequired = false } }
                },
                SupportedCurrencies = new List<string> { "JOD", "USD", "EUR" },
                SupportedLanguages = new List<string> { "en-US", "ar-JO" }
            };
        }
    }
}