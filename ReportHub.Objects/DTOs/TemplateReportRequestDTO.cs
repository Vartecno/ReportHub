using System.ComponentModel.DataAnnotations;
using ReportHub.Objects.DTOs.Common;
using ReportHub.Objects.HelperModel;

namespace ReportHub.Objects.DTOs
{
    /// <summary>
    /// Enhanced request model for template-based report generation
    /// Supports 50+ report types with dynamic branding
    /// </summary>
    public class TemplateReportRequestDTO
    {
        /// <summary>
        /// Report template identifier (e.g., "venues_invoice", "account_statement")
        /// </summary>
        [Required]
        public string ReportType { get; set; } = string.Empty;

        /// <summary>
        /// Company information and branding assets
        /// </summary>
        [Required]
        public ReportBrandingDTO Branding { get; set; } = new();

        /// <summary>
        /// Report-specific data structure
        /// </summary>
        [Required]
        public object Data { get; set; } = new();

        /// <summary>
        /// Report configuration and settings
        /// </summary>
        public ReportConfigurationDTO Configuration { get; set; } = new();

        /// <summary>
        /// Optional styling overrides for customization
        /// </summary>
        public Dictionary<string, object> StylingOverrides { get; set; } = new();

        /// <summary>
        /// Additional metadata for the report
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Branding and visual identity for reports
    /// </summary>
    public class ReportBrandingDTO
    {
        /// <summary>
        /// Company information
        /// </summary>
        [Required]
        public CompanyInfoDTO Company { get; set; } = new();

        /// <summary>
        /// Main company logo (base64 string or URL)
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// Additional icons for various report sections
        /// </summary>
        public Dictionary<string, string> Icons { get; set; } = new();

        /// <summary>
        /// Brand colors in hex format
        /// </summary>
        public BrandColorsDTO Colors { get; set; } = new();

        /// <summary>
        /// Typography settings
        /// </summary>
        public TypographyDTO Typography { get; set; } = new();
    }

    /// <summary>
    /// Enhanced company information
    /// </summary>
    public class CompanyInfoDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalInfo { get; set; } = new();
    }

    /// <summary>
    /// Brand color scheme
    /// </summary>
    public class BrandColorsDTO
    {
        public string Primary { get; set; } = "#0066CC";
        public string Secondary { get; set; } = "#F0F0F0";
        public string Accent { get; set; } = "#FF6600";
        public string Text { get; set; } = "#333333";
        public string Background { get; set; } = "#FFFFFF";
        public Dictionary<string, string> Custom { get; set; } = new();
    }

    /// <summary>
    /// Typography configuration
    /// </summary>
    public class TypographyDTO
    {
        public string FontFamily { get; set; } = "Arial";
        public int HeaderSize { get; set; } = 18;
        public int BodySize { get; set; } = 11;
        public int SmallSize { get; set; } = 9;
        public Dictionary<string, object> CustomFonts { get; set; } = new();
    }

    /// <summary>
    /// Report configuration options
    /// </summary>
    public class ReportConfigurationDTO
    {
        /// <summary>
        /// Report title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Report subtitle
        /// </summary>
        public string Subtitle { get; set; } = string.Empty;

        /// <summary>
        /// Report generation date
        /// </summary>
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Report author/generator
        /// </summary>
        public string Author { get; set; } = "System Generated";

        /// <summary>
        /// Include page numbers
        /// </summary>
        public bool IncludePageNumbers { get; set; } = true;

        /// <summary>
        /// Include header and footer
        /// </summary>
        public bool IncludeHeader { get; set; } = true;
        public bool IncludeFooter { get; set; } = true;

        /// <summary>
        /// Language and culture settings
        /// </summary>
        public string Culture { get; set; } = "en-US";
        public bool IsRightToLeft { get; set; } = false;

        /// <summary>
        /// Custom configuration options
        /// </summary>
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    /// <summary>
    /// Template-specific data structure for Venues Invoice
    /// </summary>
    public class VenuesInvoiceDataDTO
    {
        public InvoiceHeaderDTO Header { get; set; } = new();
        public ShipmentDetailsDTO Shipment { get; set; } = new();
        public List<ChargeItemDTO> Charges { get; set; } = new();
        public InvoiceTotalDTO Total { get; set; } = new();
        public List<BankAccountDTO> BankAccounts { get; set; } = new();
        public Dictionary<string, object> AdditionalFields { get; set; } = new();
    }

    /// <summary>
    /// Template-specific data structure for Account Statement
    /// </summary>
    public class AccountStatementDataDTO
    {
        public StatementHeaderDTO Header { get; set; } = new();
        public List<TransactionDTO> Transactions { get; set; } = new();
        public StatementSummaryDTO Summary { get; set; } = new();
        public Dictionary<string, object> AdditionalFields { get; set; } = new();
    }

    /// <summary>
    /// Invoice header information
    /// </summary>
    public class InvoiceHeaderDTO
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public Dictionary<string, string> CustomFields { get; set; } = new();
    }

    /// <summary>
    /// Shipment details for logistics reports
    /// </summary>
    public class ShipmentDetailsDTO
    {
        public string Service { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string AWB { get; set; } = string.Empty;
        public string MBL { get; set; } = string.Empty;
        public string HBL { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Volume { get; set; } = string.Empty;
        public Dictionary<string, object> CustomFields { get; set; } = new();
    }

    /// <summary>
    /// Charge/line item
    /// </summary>
    public class ChargeItemDTO
    {
        public int Sequence { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal RatePerUnit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, object> CustomFields { get; set; } = new();
    }

    /// <summary>
    /// Invoice totals
    /// </summary>
    public class InvoiceTotalDTO
    {
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, decimal> CurrencyTotals { get; set; } = new();
    }

    /// <summary>
    /// Bank account information
    /// </summary>
    public class BankAccountDTO
    {
        public string BankName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string SWIFT { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalInfo { get; set; } = new();
    }

    /// <summary>
    /// Account statement header
    /// </summary>
    public class StatementHeaderDTO
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, string> CustomFields { get; set; } = new();
    }

    /// <summary>
    /// Transaction record
    /// </summary>
    public class TransactionDTO
    {
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, object> AdditionalFields { get; set; } = new();
    }

    /// <summary>
    /// Statement summary
    /// </summary>
    public class StatementSummaryDTO
    {
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal ClosingBalance { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, decimal> AdditionalTotals { get; set; } = new();
    }
}