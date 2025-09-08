using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.Interfaces;
using System;

namespace ReportHub.Objects.Templates
{
    public class AccountStatementTemplate : BaseReportTemplate
    {
        public override string TemplateId => "account_statement";
        public override string DisplayName => "Account Statement";
        public override string Description => "Template for generating account statement reports with transaction history";
        public override Type[] SupportedDataTypes => new[] { typeof(AccountStatementDataDTO) };

        public override void GenerateContent(IContainer container, TemplateReportRequestDTO request)
        {
            var data = ParseTemplateData<AccountStatementDataDTO>(request.Data);
            if (data == null) return;
            
            container.Column(column =>
            {
                // Render company header using branding
                column.Item().Element(headerContainer => RenderCompanyHeader(headerContainer, request.Branding));
                
                column.Item().PaddingVertical(20).LineHorizontal(1);

                // Statement Header
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(headerColumn =>
                    {
                        headerColumn.Item().Text("ACCOUNT STATEMENT")
                            .FontSize(request.Branding.Typography.HeaderSize)
                            .Bold();
                            
                        headerColumn.Item().PaddingTop(5).Text($"Account: {data.Header.AccountName}")
                            .FontSize(request.Branding.Typography.BodySize);
                            
                        headerColumn.Item().Text($"Account No: {data.Header.AccountNumber}")
                            .FontSize(request.Branding.Typography.BodySize);
                            
                        headerColumn.Item().Text($"Period: {FormatDate(data.Header.FromDate, request.Configuration.Culture)} - {FormatDate(data.Header.ToDate, request.Configuration.Culture)}")
                            .FontSize(request.Branding.Typography.BodySize);
                    });
                });

                column.Item().PaddingVertical(20);

                // Account Summary
                column.Item().Text("ACCOUNT SUMMARY")
                    .FontSize(request.Branding.Typography.BodySize)
                    .SemiBold();
                    
                column.Item().PaddingVertical(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Cell().Element(CellStyle).Text("Opening Balance:");
                    table.Cell().Element(CellStyle).Text(FormatCurrency(data.Summary.OpeningBalance, data.Header.Currency));
                    table.Cell().Element(CellStyle).Text("Total Debits:");
                    table.Cell().Element(CellStyle).Text(FormatCurrency(data.Summary.TotalDebits, data.Header.Currency));
                    table.Cell().Element(CellStyle).Text("Total Credits:");
                    table.Cell().Element(CellStyle).Text(FormatCurrency(data.Summary.TotalCredits, data.Header.Currency));
                    table.Cell().Element(CellStyle).Text("Closing Balance:");
                    table.Cell().Element(CellStyle).Text(FormatCurrency(data.Summary.ClosingBalance, data.Header.Currency));
                });

                column.Item().PaddingVertical(20);

                // Transaction History
                if (data.Transactions?.Any() == true)
                {
                    column.Item().Text("TRANSACTION HISTORY")
                        .FontSize(request.Branding.Typography.BodySize)
                        .SemiBold();

                    column.Item().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // Date
                            columns.RelativeColumn(2); // Description
                            columns.RelativeColumn(1); // Reference
                            columns.RelativeColumn(1); // Debit
                            columns.RelativeColumn(1); // Credit
                            columns.RelativeColumn(1); // Balance
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("Date");
                            header.Cell().Element(HeaderCellStyle).Text("Description");
                            header.Cell().Element(HeaderCellStyle).Text("Reference");
                            header.Cell().Element(HeaderCellStyle).Text("Debit");
                            header.Cell().Element(HeaderCellStyle).Text("Credit");
                            header.Cell().Element(HeaderCellStyle).Text("Balance");
                        });

                        // Rows
                        foreach (var transaction in data.Transactions)
                        {
                            table.Cell().Element(CellStyle).Text(FormatDate(transaction.Date, request.Configuration.Culture));
                            table.Cell().Element(CellStyle).Text(transaction.Description);
                            table.Cell().Element(CellStyle).Text(transaction.Reference);
                            table.Cell().Element(CellStyle).Text(transaction.Debit > 0 ? FormatCurrency(transaction.Debit, transaction.Currency) : "");
                            table.Cell().Element(CellStyle).Text(transaction.Credit > 0 ? FormatCurrency(transaction.Credit, transaction.Currency) : "");
                            table.Cell().Element(CellStyle).Text(FormatCurrency(transaction.Balance, transaction.Currency));
                        }
                    });
                }

                // Footer
                column.Item().PaddingTop(30).Element(footerContainer => RenderFooter(footerContainer, request.Branding));

                // Statement Footer
                column.Item().PaddingTop(20).AlignCenter().Text("*** END OF STATEMENT ***")
                    .FontSize(request.Branding.Typography.SmallSize)
                    .Italic();
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
            return new AccountStatementDataDTO
            {
                Header = new StatementHeaderDTO
                {
                    AccountNumber = "1234567890",
                    AccountName = "Sample Account",
                    FromDate = DateTime.Now.AddDays(-30),
                    ToDate = DateTime.Now,
                    Currency = "JOD"
                },
                Transactions = new List<TransactionDTO>
                {
                    new TransactionDTO
                    {
                        Date = DateTime.Now.AddDays(-25),
                        Reference = "TXN001",
                        Description = "Opening Balance",
                        Credit = 1000.00m,
                        Balance = 1000.00m,
                        Currency = "JOD"
                    },
                    new TransactionDTO
                    {
                        Date = DateTime.Now.AddDays(-20),
                        Reference = "TXN002",
                        Description = "Payment Received",
                        Credit = 500.00m,
                        Balance = 1500.00m,
                        Currency = "JOD"
                    },
                    new TransactionDTO
                    {
                        Date = DateTime.Now.AddDays(-15),
                        Reference = "TXN003",
                        Description = "Service Charge",
                        Debit = 25.00m,
                        Balance = 1475.00m,
                        Currency = "JOD"
                    }
                },
                Summary = new StatementSummaryDTO
                {
                    OpeningBalance = 1000.00m,
                    TotalCredits = 500.00m,
                    TotalDebits = 25.00m,
                    ClosingBalance = 1475.00m
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
                    ["header"] = new FieldSchema { Name = "header", Type = "object", Description = "Statement header information", IsRequired = true },
                    ["transactions"] = new FieldSchema { Name = "transactions", Type = "array", Description = "List of transactions", IsRequired = true },
                    ["summary"] = new FieldSchema { Name = "summary", Type = "object", Description = "Statement summary", IsRequired = true }
                },
                OptionalFields = new Dictionary<string, FieldSchema>
                {
                    ["additionalFields"] = new FieldSchema { Name = "additionalFields", Type = "object", Description = "Additional fields", IsRequired = false }
                },
                SupportedCurrencies = new List<string> { "JOD", "USD", "EUR" },
                SupportedLanguages = new List<string> { "en-US", "ar-JO" }
            };
        }
    }
}