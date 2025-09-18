using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ReportHub.Objects.DTOs;
using System;

namespace ReportHub.Common.Templates
{
    public class SalesInvoiceTemplate : BaseReportTemplate
    {
        public override string TemplateId => "sales_invoice";
        public override string DisplayName => "Sales Invoice";
        public override string Description => "Template for generating Sales invoice reports with shipment details and charges";
        public override Type[] SupportedDataTypes => new[] { typeof(SalesInvoiceDataDTO) };

        public override void GenerateContent(IContainer container, TemplateReportRequestDTO request)
        {
            var data = ParseTemplateData<SalesInvoiceDataDTO>(request.Data);
            if (data == null) return;
            
            container.Column(column =>
            {
                // Render company header using branding
                column.Item().Element(headerContainer => RenderCompanyHeader(headerContainer, request.Branding));
                
                column.Item().PaddingVertical(20).LineHorizontal(1);

                // Invoice Header
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(invoiceColumn =>
                    {
                        invoiceColumn.Item().Text("INVOICE")
                            .FontSize(request.Branding.Typography.HeaderSize)
                            .Bold();
                            
                        invoiceColumn.Item().PaddingTop(5).Text($"Invoice #{data.Header.InvoiceNumber}")
                            .FontSize(request.Branding.Typography.BodySize);
                            
                        invoiceColumn.Item().Text($"Date: {FormatDate(data.Header.Date, request.Configuration.Culture)}")
                            .FontSize(request.Branding.Typography.BodySize);
                            
                        if (!string.IsNullOrEmpty(data.Header.Reference))
                            invoiceColumn.Item().Text($"Reference: {data.Header.Reference}")
                                .FontSize(request.Branding.Typography.BodySize);
                    });

                    row.RelativeItem().Column(customerColumn =>
                    {
                        customerColumn.Item().Text("BILL TO:")
                            .FontSize(request.Branding.Typography.BodySize)
                            .SemiBold();
                            
                        customerColumn.Item().Text(data.Header.CustomerName)
                            .FontSize(request.Branding.Typography.BodySize);
                            
                        if (!string.IsNullOrEmpty(data.Header.CustomerAddress))
                            customerColumn.Item().Text(data.Header.CustomerAddress)
                                .FontSize(request.Branding.Typography.BodySize);
                    });
                });

                column.Item().PaddingVertical(20);

                // Shipment Details
                column.Item().Text("SHIPMENT DETAILS")
                    .FontSize(request.Branding.Typography.BodySize)
                    .SemiBold();
                    
                column.Item().PaddingVertical(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Cell().Element(CellStyle).Text("Service:");
                    table.Cell().Element(CellStyle).Text(data.Shipment.Service);
                    table.Cell().Element(CellStyle).Text("Origin:");
                    table.Cell().Element(CellStyle).Text(data.Shipment.Origin);
                    table.Cell().Element(CellStyle).Text("Destination:");
                    table.Cell().Element(CellStyle).Text(data.Shipment.Destination);
                    
                    if (!string.IsNullOrEmpty(data.Shipment.AWB))
                    {
                        table.Cell().Element(CellStyle).Text("AWB:");
                        table.Cell().Element(CellStyle).Text(data.Shipment.AWB);
                    }
                    
                    if (!string.IsNullOrEmpty(data.Shipment.MBL))
                    {
                        table.Cell().Element(CellStyle).Text("MBL:");
                        table.Cell().Element(CellStyle).Text(data.Shipment.MBL);
                    }
                    
                    if (!string.IsNullOrEmpty(data.Shipment.HBL))
                    {
                        table.Cell().Element(CellStyle).Text("HBL:");
                        table.Cell().Element(CellStyle).Text(data.Shipment.HBL);
                    }
                });

                column.Item().PaddingVertical(20);

                // Charges Table
                if (data.Charges?.Any() == true)
                {
                    column.Item().Text("CHARGES")
                        .FontSize(request.Branding.Typography.BodySize)
                        .SemiBold();

                    column.Item().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);   // Sequence
                            columns.RelativeColumn(3);    // Description
                            columns.RelativeColumn(1);    // Rate
                            columns.RelativeColumn(1);    // Quantity
                            columns.RelativeColumn(1);    // Total
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("Seq");
                            header.Cell().Element(HeaderCellStyle).Text("Description");
                            header.Cell().Element(HeaderCellStyle).Text("Rate");
                            header.Cell().Element(HeaderCellStyle).Text("Qty");
                            header.Cell().Element(HeaderCellStyle).Text("Total");
                        });

                        // Rows
                        foreach (var charge in data.Charges)
                        {
                            table.Cell().Element(CellStyle).Text(charge.Sequence.ToString());
                            table.Cell().Element(CellStyle).Text(charge.Description);
                            table.Cell().Element(CellStyle).Text(FormatCurrency(charge.RatePerUnit, charge.Currency));
                            table.Cell().Element(CellStyle).Text(charge.Quantity.ToString("N2"));
                            table.Cell().Element(CellStyle).Text(FormatCurrency(charge.Value, charge.Currency));
                        }
                    });
                }

                column.Item().PaddingVertical(20);

                // Totals
                column.Item().AlignRight().Column(totalColumn =>
                {
                    totalColumn.Item().Text($"Subtotal: {FormatCurrency(data.Total.Subtotal, data.Total.Currency)}")
                        .FontSize(request.Branding.Typography.BodySize);
                        
                    if (data.Total.Tax > 0)
                        totalColumn.Item().Text($"Tax: {FormatCurrency(data.Total.Tax, data.Total.Currency)}")
                            .FontSize(request.Branding.Typography.BodySize);
                            
                    totalColumn.Item().Text($"Total: {FormatCurrency(data.Total.Total, data.Total.Currency)}")
                        .FontSize(request.Branding.Typography.HeaderSize)
                        .Bold();
                });

                // Bank Accounts
                if (data.BankAccounts?.Any() == true)
                {
                    column.Item().PaddingVertical(20).Column(bankColumn =>
                    {
                        bankColumn.Item().Text("BANK DETAILS")
                            .FontSize(request.Branding.Typography.BodySize)
                            .SemiBold();

                        foreach (var bank in data.BankAccounts)
                        {
                            bankColumn.Item().PaddingTop(10).Column(bankInfoColumn =>
                            {
                                bankInfoColumn.Item().Text($"Bank: {bank.BankName}")
                                    .FontSize(request.Branding.Typography.SmallSize);
                                bankInfoColumn.Item().Text($"Account: {bank.AccountName}")
                                    .FontSize(request.Branding.Typography.SmallSize);
                                bankInfoColumn.Item().Text($"Account No: {bank.AccountNumber}")
                                    .FontSize(request.Branding.Typography.SmallSize);
                                    
                                if (!string.IsNullOrEmpty(bank.IBAN))
                                    bankInfoColumn.Item().Text($"IBAN: {bank.IBAN}")
                                        .FontSize(request.Branding.Typography.SmallSize);
                                        
                                if (!string.IsNullOrEmpty(bank.SWIFT))
                                    bankInfoColumn.Item().Text($"SWIFT: {bank.SWIFT}")
                                        .FontSize(request.Branding.Typography.SmallSize);
                            });
                        }
                    });
                }

                // Footer
                column.Item().PaddingTop(30).Element(footerContainer => RenderFooter(footerContainer, request.Branding));
            });
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor("#CCCCCC")
                .Padding(8)
                .AlignMiddle();
        }

        private static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor("#CCCCCC")
                .Background("#F0F0F0")
                .Padding(8)
                .AlignMiddle();
        }

        public override object GetSampleData()
        {
            return new SalesInvoiceDataDTO
            {
                Header = new InvoiceHeaderDTO
                {
                    InvoiceNumber = "INV-001",
                    Date = DateTime.Now,
                    Reference = "REF-12345",
                    CustomerName = "Sample Customer Ltd.",
                    CustomerAddress = "123 Sample Street, Sample City"
                },
                Shipment = new ShipmentDetailsDTO
                {
                    Service = "Air Freight",
                    Origin = "Amman, Jordan",
                    Destination = "Dubai, UAE",
                    AWB = "123-45678901",
                    Weight = 100.5m,
                    Volume = "2.5 CBM"
                },
                Charges = new List<ChargeItemDTO>
                {
                    new ChargeItemDTO { Sequence = 1, Description = "Air Freight", RatePerUnit = 5.0m, Quantity = 100.5m, Value = 502.5m, Currency = "JOD" },
                    new ChargeItemDTO { Sequence = 2, Description = "Documentation Fee", RatePerUnit = 25.0m, Quantity = 1m, Value = 25.0m, Currency = "JOD" }
                },
                Total = new InvoiceTotalDTO
                {
                    Subtotal = 527.5m,
                    Tax = 52.75m,
                    Total = 580.25m,
                    Currency = "JOD"
                },
                BankAccounts = new List<BankAccountDTO>
                {
                    new BankAccountDTO
                    {
                        BankName = "Jordan Bank",
                        AccountName = "Company Name",
                        AccountNumber = "123456789",
                        IBAN = "JO15CBJO0010000000000131000302",
                        SWIFT = "CBJOJOAX",
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
                    ["header"] = new FieldSchema { Name = "header", Type = "object", Description = "Invoice header information", IsRequired = true },
                    ["shipment"] = new FieldSchema { Name = "shipment", Type = "object", Description = "Shipment details", IsRequired = true },
                    ["charges"] = new FieldSchema { Name = "charges", Type = "array", Description = "List of charges", IsRequired = true },
                    ["total"] = new FieldSchema { Name = "total", Type = "object", Description = "Invoice totals", IsRequired = true }
                },
                OptionalFields = new Dictionary<string, FieldSchema>
                {
                    ["bankAccounts"] = new FieldSchema { Name = "bankAccounts", Type = "array", Description = "Bank account details", IsRequired = false }
                },
                SupportedCurrencies = new List<string> { "JOD", "USD", "EUR" },
                SupportedLanguages = new List<string> { "en-US", "ar-JO" }
            };
        }
    }
}
