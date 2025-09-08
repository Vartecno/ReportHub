using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.Interfaces;
using Colors = QuestPDF.Helpers.Colors;

namespace ReportHub.Objects.Templates
{
    /// <summary>
    /// Template for Account Statement reports
    /// Matches the layout of the second PDF sample
    /// </summary>
    public class AccountStatementTemplate : BaseReportTemplate
    {
        public override string TemplateId => "account_statement";
        public override string DisplayName => "Account Statement";
        public override string Description => "Professional account statement template for financial transactions";
        public override Type[] SupportedDataTypes => new[] { typeof(AccountStatementDataDTO) };

        public override IContainer GenerateContent(TemplateReportRequestDTO request)
        {
            var statementData = ParseTemplateData<AccountStatementDataDTO>(request.Data);
            if (statementData == null)
                throw new ArgumentException("Invalid data format for AccountStatementTemplate");

            return container =>
            {
                container.Column(column =>
                {
                    // Header Section
                    column.Item().PaddingBottom(20).Element(headerContainer => 
                        RenderStatementHeader(headerContainer, request.Branding, statementData.Header));

                    // Statement Title (centered)
                    column.Item().PaddingBottom(15).AlignCenter().Text("Account Statements")
                        .FontSize(18).SemiBold().FontColor(request.Branding.Colors.Primary);

                    // Date Range
                    column.Item().PaddingBottom(15).Text($"From Date: {FormatDate(statementData.Header.FromDate)} To Date: {FormatDate(statementData.Header.ToDate)}")
                        .FontSize(request.Branding.Typography.BodySize);

                    // Transactions Table
                    if (statementData.Transactions?.Any() == true)
                    {
                        column.Item().PaddingBottom(15).Element(transactionsContainer => 
                            RenderTransactionsTable(transactionsContainer, statementData.Transactions, request.Branding));
                    }

                    // Summary Section
                    if (statementData.Summary != null)
                    {
                        column.Item().PaddingBottom(15).Element(summaryContainer => 
                            RenderSummary(summaryContainer, statementData.Summary, request.Branding));
                    }

                    // Footer with contact information
                    column.Item().Element(footerContainer => 
                        RenderContactFooter(footerContainer, request.Branding));
                });
            };
        }

        private void RenderStatementHeader(IContainer container, ReportBrandingDTO branding, StatementHeaderDTO header)
        {
            container.Column(column =>
            {
                // City and Company Name
                if (!string.IsNullOrWhiteSpace(branding.Company.City))
                {
                    column.Item().Text(branding.Company.City)
                        .FontSize(branding.Typography.BodySize);
                }

                column.Item().PaddingTop(5).Row(row =>
                {
                    // Logo section
                    if (!string.IsNullOrWhiteSpace(branding.Logo))
                    {
                        row.ConstantItem(120).Element(logoContainer => RenderLogo(logoContainer, branding.Logo, 120, 60));
                        row.RelativeItem().PaddingLeft(15);
                    }
                    else
                    {
                        row.RelativeItem();
                    }

                    // Company name
                    row.RelativeItem().Column(nameColumn =>
                    {
                        nameColumn.Item().Text(branding.Company.Name)
                            .FontSize(14).SemiBold();
                    });
                });
            });
        }

        private void RenderTransactionsTable(IContainer container, List<TransactionDTO> transactions, ReportBrandingDTO branding)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(80);   // Date
                    columns.ConstantColumn(100);  // Account No
                    columns.RelativeColumn(2);    // Account Name
                    columns.RelativeColumn(2);    // Notes
                    columns.ConstantColumn(120);  // Supplier Invoice No
                    columns.ConstantColumn(100);  // Invoice No
                    columns.ConstantColumn(100);  // Reference
                    columns.ConstantColumn(100);  // Debit
                    columns.ConstantColumn(100);  // Credit
                    columns.ConstantColumn(100);  // Balance
                });

                // Header row - concatenated style from original
                table.Cell().ColumnSpan(10).Background(branding.Colors.Primary).Padding(5)
                    .Text("DateAccount NoAccountNameNotesSupplier Invoice NoInvoice No.ReferenceDebitCreditBalance")
                    .FontSize(10).SemiBold().FontColor(Colors.White);

                // Data rows
                for (int i = 0; i < transactions.Count; i++)
                {
                    var transaction = transactions[i];
                    var isEvenRow = i % 2 == 0;
                    var bgColor = isEvenRow ? Colors.White : Colors.Grey.Lighten4;

                    // Date
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(FormatDate(transaction.Date)).FontSize(9);

                    // Account fields - simplified for demo
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(transaction.AdditionalFields.GetValueOrDefault("AccountNo", "").ToString()).FontSize(9);
                    
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(transaction.AdditionalFields.GetValueOrDefault("AccountName", "").ToString()).FontSize(9);
                    
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(transaction.Description).FontSize(9);
                    
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(transaction.AdditionalFields.GetValueOrDefault("SupplierInvoiceNo", "").ToString()).FontSize(9);
                    
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(transaction.AdditionalFields.GetValueOrDefault("InvoiceNo", "").ToString()).FontSize(9);
                    
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(transaction.Reference).FontSize(9);

                    // Debit
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(transaction.Debit > 0 ? FormatCurrency(transaction.Debit, transaction.Currency) : "")
                        .FontSize(9);

                    // Credit
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(transaction.Credit > 0 ? FormatCurrency(transaction.Credit, transaction.Currency) : "")
                        .FontSize(9);

                    // Balance
                    var balanceText = FormatCurrency(Math.Abs(transaction.Balance), transaction.Currency);
                    if (transaction.Balance < 0)
                        balanceText = $"({balanceText})";
                    
                    table.Cell().Background(bgColor).Padding(3)
                        .Text(balanceText).FontSize(9);
                }
            });
        }

        private void RenderSummary(IContainer container, StatementSummaryDTO summary, ReportBrandingDTO branding)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(10).Text("Summary")
                    .FontSize(12).SemiBold();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                    });

                    // Summary rows
                    AddSummaryRow(table, "Opening Balance", FormatCurrency(summary.OpeningBalance, summary.Currency));
                    AddSummaryRow(table, "Total Debits", FormatCurrency(summary.TotalDebits, summary.Currency));
                    AddSummaryRow(table, "Total Credits", FormatCurrency(summary.TotalCredits, summary.Currency));
                    AddSummaryRow(table, "Closing Balance", FormatCurrency(summary.ClosingBalance, summary.Currency));
                });
            });
        }

        private void AddSummaryRow(TableDescriptor table, string label, string value)
        {
            table.Cell().Padding(5).Text(label).FontSize(10).SemiBold();
            table.Cell().Padding(5).Text(value).FontSize(10);
        }

        private void RenderContactFooter(IContainer container, ReportBrandingDTO branding)
        {
            container.PaddingTop(20).Column(column =>
            {
                // Contact information block
                column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(10);
                
                if (!string.IsNullOrWhiteSpace(branding.Company.Address))
                {
                    column.Item().Text(branding.Company.Address)
                        .FontSize(branding.Typography.SmallSize)
                        .FontColor(Colors.Grey.Medium);
                }
                
                if (!string.IsNullOrWhiteSpace(branding.Company.Phone))
                {
                    column.Item().Text(branding.Company.Phone)
                        .FontSize(branding.Typography.SmallSize)
                        .FontColor(Colors.Grey.Medium);
                }
                
                if (!string.IsNullOrWhiteSpace(branding.Company.Email))
                {
                    column.Item().Text(branding.Company.Email)
                        .FontSize(branding.Typography.SmallSize)
                        .FontColor(Colors.Grey.Medium);
                }
            });
        }

        public override TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request)
        {
            var result = base.ValidateRequest(request);
            
            if (result.IsValid)
            {
                var statementData = ParseTemplateData<AccountStatementDataDTO>(request.Data);
                if (statementData == null)
                {
                    result.Errors.Add("Data must be of type AccountStatementDataDTO");
                }
                else
                {
                    if (statementData.Header == null)
                        result.Errors.Add("Statement header is required");
                    if (statementData.Transactions == null || !statementData.Transactions.Any())
                        result.Warnings.Add("No transactions found in statement");
                }
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }

        public override object GetSampleData()
        {
            return new AccountStatementDataDTO
            {
                Header = new StatementHeaderDTO
                {
                    AccountNumber = "123456789",
                    AccountName = "Main Account",
                    FromDate = DateTime.Now.AddMonths(-1),
                    ToDate = DateTime.Now,
                    Currency = "JOD"
                },
                Transactions = new List<TransactionDTO>
                {
                    new TransactionDTO
                    {
                        Date = DateTime.Now.AddDays(-15),
                        Reference = "REF001",
                        Description = "Payment received",
                        Debit = 0,
                        Credit = 1000.00m,
                        Balance = 1000.00m,
                        Currency = "JOD",
                        AdditionalFields = new Dictionary<string, object>
                        {
                            { "AccountNo", "123456" },
                            { "AccountName", "Sample Account" },
                            { "SupplierInvoiceNo", "" },
                            { "InvoiceNo", "INV001" }
                        }
                    },
                    new TransactionDTO
                    {
                        Date = DateTime.Now.AddDays(-10),
                        Reference = "REF002",
                        Description = "Service fee",
                        Debit = 50.00m,
                        Credit = 0,
                        Balance = 950.00m,
                        Currency = "JOD",
                        AdditionalFields = new Dictionary<string, object>
                        {
                            { "AccountNo", "123456" },
                            { "AccountName", "Sample Account" },
                            { "SupplierInvoiceNo", "SUPP001" },
                            { "InvoiceNo", "" }
                        }
                    }
                },
                Summary = new StatementSummaryDTO
                {
                    OpeningBalance = 0m,
                    TotalDebits = 50.00m,
                    TotalCredits = 1000.00m,
                    ClosingBalance = 950.00m,
                    Currency = "JOD"
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
                    { "Header", new FieldSchema { Name = "Header", Type = "StatementHeaderDTO", Description = "Statement header information", IsRequired = true } },
                    { "Transactions", new FieldSchema { Name = "Transactions", Type = "List<TransactionDTO>", Description = "List of transactions", IsRequired = true } }
                },
                OptionalFields = new Dictionary<string, FieldSchema>
                {
                    { "Summary", new FieldSchema { Name = "Summary", Type = "StatementSummaryDTO", Description = "Statement summary totals", IsRequired = false } }
                },
                SupportedCurrencies = new List<string> { "JOD", "USD", "EUR" },
                SupportedLanguages = new List<string> { "en-US", "ar-JO" }
            };
        }
    }
}