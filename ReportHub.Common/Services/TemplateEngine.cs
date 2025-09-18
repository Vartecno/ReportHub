using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.HelperModel;
using ReportHub.Objects.Interfaces;
using System.Collections.Concurrent;

namespace ReportHub.Common.Services
{
    
    public class TemplateEngine : ITemplateEngine
    {
        private readonly ConcurrentDictionary<string, IReportTemplate> _templates = new();
        private readonly ILogger<TemplateEngine>? _logger;

        public TemplateEngine(ILogger<TemplateEngine>? logger = null)
        {
            // Configure QuestPDF license for Community use
            QuestPDF.Settings.License = LicenseType.Community;
            _logger = logger;
        }
        
        public void RegisterTemplate(IReportTemplate template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (string.IsNullOrWhiteSpace(template.TemplateId))
                throw new ArgumentException("Template must have a valid TemplateId", nameof(template));

            _templates.AddOrUpdate(template.TemplateId, template, (key, existing) =>
            {
                _logger?.LogWarning("Template {TemplateId} is being replaced", template.TemplateId);
                return template;
            });

            _logger?.LogInformation("Template {TemplateId} ({DisplayName}) registered successfully", 
                template.TemplateId, template.DisplayName);
        }
        
        public Dictionary<string, IReportTemplate> GetRegisteredTemplates()
        {
            return new Dictionary<string, IReportTemplate>(_templates);
        }
        
        public bool IsTemplateRegistered(string templateId)
        {
            return !string.IsNullOrWhiteSpace(templateId) && _templates.ContainsKey(templateId);
        }
        
        public IReportTemplate? GetTemplate(string templateId)
        {
            if (string.IsNullOrWhiteSpace(templateId))
                return null;

            _templates.TryGetValue(templateId, out var template);
            return template;
        }
        
        public async Task<ReportResult> GenerateReportAsync(TemplateReportRequestDTO request)
        {
            try
            {
                _logger?.LogInformation("Generating report for template: {TemplateId}", request.ReportType);

                // Validate request
                var validationResult = ValidateRequest(request);
                if (!validationResult.IsValid)
                {
                    return new ReportResult
                    {
                        Success = false,
                        Errors = validationResult.Errors
                    };
                }

                // Get template
                var template = GetTemplate(request.ReportType);
                if (template == null)
                {
                    return new ReportResult
                    {
                        Success = false,
                        Errors = new List<string> { $"Template '{request.ReportType}' not found" }
                    };
                }

                // Generate PDF document
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // Page settings
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(request.Branding.Typography.BodySize)
                            .FontFamily(request.Branding.Typography.FontFamily));

                        // Generate content using template
                        page.Content().Element(container => template.GenerateContent(container, request));

                        // Add footer if enabled
                        if (request.Configuration.IncludeFooter)
                        {
                            page.Footer().Element(footerContainer =>
                            {
                                footerContainer.Column(column =>
                                {
                                    column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1);
                                    column.Item().PaddingTop(10).Row(row =>
                                    {
                                        row.RelativeItem().Text($"Generated on: {request.Configuration.GeneratedDate:dd/MM/yyyy HH:mm}")
                                            .FontSize(request.Branding.Typography.SmallSize);

                                        if (request.Configuration.IncludePageNumbers)
                                        {
                                            row.ConstantItem(100).AlignRight().Column(pageColumn =>
                                            {
                                                pageColumn.Item().Text(text =>
                                                {
                                                    text.DefaultTextStyle(style => style.FontSize(request.Branding.Typography.SmallSize));
                                                    text.CurrentPageNumber();
                                                    text.Span(" / ");
                                                    text.TotalPages();
                                                });
                                            });
                                        }
                                    });
                                });
                            });
                        }
                    });
                });

                var pdfBytes = await Task.Run(() => document.GeneratePdf());
                var fileName = GenerateFileName(request.Configuration.Title, request.ReportType);

                _logger?.LogInformation("Report generated successfully: {FileName} ({Size} bytes)", 
                    fileName, pdfBytes.Length);

                return new ReportResult
                {
                    Data = pdfBytes,
                    ContentType = "application/pdf",
                    FileName = fileName,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error generating report for template: {TemplateId}", request.ReportType);

                return new ReportResult
                {
                    Success = false,
                    Errors = new List<string> { $"An error occurred while generating the report: {ex.Message}" }
                };
            }
        }
        
        public TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request)
        {
            var result = new TemplateValidationResult { IsValid = true };

            // Basic validation
            if (request == null)
            {
                result.Errors.Add("Request cannot be null");
                result.IsValid = false;
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.ReportType))
            {
                result.Errors.Add("ReportType is required");
            }
            else
            {
                var template = GetTemplate(request.ReportType);
                if (template == null)
                {
                    result.Errors.Add($"Template '{request.ReportType}' is not registered");
                }
                else
                {
                    // Delegate to template-specific validation
                    var templateValidation = template.ValidateRequest(request);
                    result.Errors.AddRange(templateValidation.Errors);
                    result.Warnings.AddRange(templateValidation.Warnings);
                }
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }
        
        public List<TemplateMetadata> GetAvailableTemplates()
        {
            return _templates.Values.Select(template => new TemplateMetadata
            {
                TemplateId = template.TemplateId,
                DisplayName = template.DisplayName,
                Description = template.Description,
                Version = template.Version,
                Category = GetTemplateCategory(template.TemplateId),
                Tags = GetTemplateTags(template.TemplateId),
                LastModified = DateTime.UtcNow, // In production, this would come from metadata
                IsActive = true,
                ConfigSchema = template.GetConfigSchema(),
                SampleData = template.GetSampleData()
            }).ToList();
        }

        private string GenerateFileName(string title, string templateId)
        {
            var safeTitle = string.IsNullOrWhiteSpace(title) ? templateId : title;
            var fileName = string.Join("_", safeTitle.Split(Path.GetInvalidFileNameChars()));
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            return $"{fileName}_{timestamp}.pdf";
        }

        private string GetTemplateCategory(string templateId)
        {
            // Categorize templates based on their ID
            if (templateId.Contains("invoice", StringComparison.OrdinalIgnoreCase))
                return "Invoices";
            if (templateId.Contains("statement", StringComparison.OrdinalIgnoreCase))
                return "Financial Statements";
            if (templateId.Contains("report", StringComparison.OrdinalIgnoreCase))
                return "Reports";
            return "General";
        }

        private List<string> GetTemplateTags(string templateId)
        {
            var tags = new List<string>();
            
            if (templateId.Contains("invoice", StringComparison.OrdinalIgnoreCase))
                tags.AddRange(new[] { "billing", "invoice", "financial" });
            if (templateId.Contains("statement", StringComparison.OrdinalIgnoreCase))
                tags.AddRange(new[] { "statement", "accounting", "financial" });
            if (templateId.Contains("sales", StringComparison.OrdinalIgnoreCase))
                tags.AddRange(new[] { "logistics", "shipping" });
            if (templateId.Contains("account", StringComparison.OrdinalIgnoreCase))
                tags.AddRange(new[] { "accounting", "transactions" });
                
            return tags;
        }
    }
}