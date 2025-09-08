using QuestPDF.Infrastructure;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;
using ReportHub.Objects.Interfaces;
using ReportHub.Common.Services;

namespace ReportHub.Common.Helper.GeneratorHelper
{
    /// <summary>
    /// Template-based PDF generator that replaces the original hardcoded PDF_GeneratorHelper
    /// Supports multiple report templates with dynamic branding
    /// </summary>
    public class TemplateBasedPDFGenerator : IFormatSpecificGenerator
    {
        private readonly ITemplateEngineService _templateEngineService;

        public TemplateBasedPDFGenerator(ITemplateEngineService templateEngineService)
        {
            _templateEngineService = templateEngineService ?? throw new ArgumentNullException(nameof(templateEngineService));
        }

        public ReportFormat SupportedFormat => ReportFormat.PDF;
        public string ContentType => "application/pdf";

        static TemplateBasedPDFGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Generate PDF using template engine
        /// Supports both legacy ReportData format and new TemplateReportRequestDTO format
        /// </summary>
        public async Task<byte[]> GenerateAsync(ReportSettings settings, ReportData data)
        {
            // Convert legacy format to template format if needed
            var templateRequest = ConvertToTemplateRequest(settings, data);
            
            var result = await _templateEngineService.GenerateTemplateReportAsync(templateRequest);
            
            if (!result.Success)
            {
                throw new InvalidOperationException($"Template generation failed: {string.Join(", ", result.Errors)}");
            }
            
            return result.Data;
        }

        public async Task<Stream> GenerateStreamAsync(ReportSettings settings, ReportData data)
        {
            var bytes = await GenerateAsync(settings, data);
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// Convert legacy ReportData format to new TemplateReportRequestDTO format
        /// Maintains backward compatibility with existing API calls
        /// </summary>
        private TemplateReportRequestDTO ConvertToTemplateRequest(ReportSettings settings, ReportData data)
        {
            // Determine template type based on data structure or settings
            var templateType = DetermineTemplateType(settings, data);
            
            return new TemplateReportRequestDTO
            {
                ReportType = templateType,
                Branding = CreateBrandingFromSettings(settings, data),
                Data = ConvertDataForTemplate(templateType, data),
                Configuration = CreateConfigurationFromSettings(settings),
                StylingOverrides = new Dictionary<string, object>(),
                Metadata = CreateMetadataFromSettings(settings)
            };
        }

        private string DetermineTemplateType(ReportSettings settings, ReportData data)
        {
            // Logic to determine which template to use based on data structure
            // This ensures backward compatibility
            
            // Check for invoice-related data
            if (data.Tables.Any(t => t.Title.Contains("Charges", StringComparison.OrdinalIgnoreCase)) ||
                data.Tables.Any(t => t.Title.Contains("Shipment", StringComparison.OrdinalIgnoreCase)) ||
                data.Variables.ContainsKey("invoiceNumber"))
            {
                return "venues_invoice";
            }
            
            // Check for statement-related data
            if (settings.Title.Contains("Statement", StringComparison.OrdinalIgnoreCase) ||
                data.Tables.Any(t => t.Headers.Contains("Debit") && t.Headers.Contains("Credit")))
            {
                return "account_statement";
            }
            
            // Default to venues_invoice for backward compatibility
            return "venues_invoice";
        }

        private ReportBrandingDTO CreateBrandingFromSettings(ReportSettings settings, ReportData data)
        {
            return new ReportBrandingDTO
            {
                Company = new CompanyInfoDTO
                {
                    Name = data.Variables.GetValueOrDefault("companyName", "Nuba Logistics").ToString(),
                    Address = "Amman-Jordan, Mecca Street, Rashed Al-Neimat Building No.9, 3rd Floor, Office No.301",
                    Phone = "+96265812954",
                    Email = "info@nubalogistic.com",
                    City = "Amman"
                },
                Logo = null, // Logo will need to be provided in the new API
                Icons = new Dictionary<string, string>(),
                Colors = new BrandColorsDTO
                {
                    Primary = settings.Formatting.PrimaryColor,
                    Secondary = settings.Formatting.SecondaryColor,
                    Text = "#333333",
                    Background = "#FFFFFF"
                },
                Typography = new TypographyDTO
                {
                    FontFamily = settings.Formatting.FontFamily,
                    HeaderSize = 18,
                    BodySize = settings.Formatting.FontSize,
                    SmallSize = 9
                }
            };
        }

        private object ConvertDataForTemplate(string templateType, ReportData data)
        {
            switch (templateType)
            {
                case "venues_invoice":
                    return ConvertToVenuesInvoiceData(data);
                case "account_statement":
                    return ConvertToAccountStatementData(data);
                default:
                    return ConvertToVenuesInvoiceData(data); // Default fallback
            }
        }

        private VenuesInvoiceDataDTO ConvertToVenuesInvoiceData(ReportData data)
        {
            var invoiceData = new VenuesInvoiceDataDTO();
            
            // Extract header information
            invoiceData.Header = new InvoiceHeaderDTO
            {
                InvoiceNumber = data.Variables.GetValueOrDefault("invoiceNumber", "").ToString(),
                Date = DateTime.Now,
                Reference = data.Variables.GetValueOrDefault("nubaReference", "").ToString(),
                CustomerName = data.Variables.GetValueOrDefault("customerName", "").ToString()
            };
            
            // Extract shipment details
            var shipmentTable = data.Tables.FirstOrDefault(t => t.Title == "Shipment Details");
            if (shipmentTable != null)
            {
                invoiceData.Shipment = new ShipmentDetailsDTO();
                foreach (var row in shipmentTable.Rows)
                {
                    if (row.Count >= 2)
                    {
                        var key = row[0].ToString().ToLower();
                        var value = row[1].ToString();
                        
                        switch (key)
                        {
                            case "service":
                                invoiceData.Shipment.Service = value;
                                break;
                            case "origin":
                                invoiceData.Shipment.Origin = value;
                                break;
                            case "destination":
                                invoiceData.Shipment.Destination = value;
                                break;
                            // Add other mappings as needed
                        }
                    }
                }
            }
            
            // Extract charges
            var chargesTable = data.Tables.FirstOrDefault(t => t.Title == "Charges");
            if (chargesTable != null)
            {
                invoiceData.Charges = new List<ChargeItemDTO>();
                for (int i = 0; i < chargesTable.Rows.Count; i++)
                {
                    var row = chargesTable.Rows[i];
                    if (row.Count >= 6)
                    {
                        invoiceData.Charges.Add(new ChargeItemDTO
                        {
                            Sequence = i + 1,
                            Description = row[1].ToString(),
                            RatePerUnit = decimal.TryParse(row[2].ToString(), out var rate) ? rate : 0,
                            Price = decimal.TryParse(row[3].ToString(), out var price) ? price : 0,
                            Quantity = decimal.TryParse(row[4].ToString(), out var qty) ? qty : 0,
                            Value = decimal.TryParse(row[5].ToString(), out var value) ? value : 0,
                            Currency = "JOD"
                        });
                    }
                }
            }
            
            // Extract total
            var totalTable = data.Tables.FirstOrDefault(t => t.Title == "Total Amount");
            if (totalTable != null && totalTable.Rows.Count > 0)
            {
                invoiceData.Total = new InvoiceTotalDTO
                {
                    Total = decimal.TryParse(totalTable.Rows[0][1].ToString(), out var total) ? total : 0,
                    Currency = "JOD"
                };
            }
            
            return invoiceData;
        }

        private AccountStatementDataDTO ConvertToAccountStatementData(ReportData data)
        {
            // Implementation for converting legacy data to account statement format
            // This is a placeholder - you would implement based on your actual data structure
            return new AccountStatementDataDTO
            {
                Header = new StatementHeaderDTO
                {
                    AccountNumber = "Legacy Account",
                    AccountName = "Converted Account",
                    FromDate = DateTime.Now.AddMonths(-1),
                    ToDate = DateTime.Now,
                    Currency = "JOD"
                },
                Transactions = new List<TransactionDTO>(),
                Summary = new StatementSummaryDTO
                {
                    Currency = "JOD"
                }
            };
        }

        private ReportConfigurationDTO CreateConfigurationFromSettings(ReportSettings settings)
        {
            return new ReportConfigurationDTO
            {
                Title = settings.Title,
                Subtitle = settings.Subtitle,
                GeneratedDate = settings.CreatedDate,
                Author = settings.Author,
                IncludePageNumbers = settings.Formatting.IncludePageNumbers,
                IncludeHeader = settings.Formatting.IncludeHeader,
                IncludeFooter = settings.Formatting.IncludeFooter,
                Culture = settings.Culture
            };
        }

        private Dictionary<string, object> CreateMetadataFromSettings(ReportSettings settings)
        {
            return settings.Metadata.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
        }
    }
}