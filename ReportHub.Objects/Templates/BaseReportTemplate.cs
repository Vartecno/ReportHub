using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.Interfaces;
using System.Globalization;
using System.Text.Json;
using Colors = QuestPDF.Helpers.Colors;

namespace ReportHub.Objects.Templates
{
    /// <summary>
    /// Abstract base class for all report templates
    /// Provides common functionality and components
    /// </summary>
    public abstract class BaseReportTemplate : IReportTemplate
    {
        public abstract string TemplateId { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public virtual string Version => "1.0.0";
        public abstract Type[] SupportedDataTypes { get; }

        public abstract void GenerateContent(IContainer container, TemplateReportRequestDTO request);
        public abstract object GetSampleData();
        public abstract TemplateConfigSchema GetConfigSchema();

        /// <summary>
        /// Base validation for all templates
        /// </summary>
        public virtual TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request)
        {
            var result = new TemplateValidationResult { IsValid = true };

            // Basic validation
            if (string.IsNullOrWhiteSpace(request.ReportType))
            {
                result.Errors.Add("ReportType is required");
            }

            if (request.ReportType != TemplateId)
            {
                result.Errors.Add($"ReportType '{request.ReportType}' does not match template '{TemplateId}'");
            }

            if (request.Branding?.Company == null)
            {
                result.Errors.Add("Company information is required");
            }
            else if (string.IsNullOrWhiteSpace(request.Branding.Company.Name))
            {
                result.Errors.Add("Company name is required");
            }

            if (request.Data == null)
            {
                result.Errors.Add("Data is required");
            }

            result.IsValid = !result.Errors.Any();
            return result;
        }

        #region Protected Helper Methods

        /// <summary>
        /// Render company logo from base64 string or URL
        /// </summary>
        protected void RenderLogo(IContainer container, string? logoData, float maxWidth = 150, float maxHeight = 80)
        {
            if (string.IsNullOrWhiteSpace(logoData))
                return;

            try
            {
                if (logoData.StartsWith("data:image/"))
                {
                    // Base64 image
                    var base64Data = logoData.Split(',')[1];
                    var imageBytes = Convert.FromBase64String(base64Data);
                    container.Width(maxWidth).Height(maxHeight).Image(imageBytes).FitArea();
                }
                else if (Uri.IsWellFormedUriString(logoData, UriKind.Absolute))
                {
                    // URL image - in production, you'd want to download and cache
                    // For now, we'll show placeholder
                    container.Width(maxWidth).Height(maxHeight)
                        .Border(1).BorderColor(Colors.Grey.Lighten2)
                        .AlignCenter().AlignMiddle()
                        .Text("Logo")
                        .FontSize(12).FontColor(Colors.Grey.Medium);
                }
            }
            catch
            {
                // Fallback: show placeholder
                container.Width(maxWidth).Height(maxHeight)
                    .Border(1).BorderColor(Colors.Grey.Lighten2)
                    .AlignCenter().AlignMiddle()
                    .Text("Logo")
                    .FontSize(12).FontColor(Colors.Grey.Medium);
            }
        }

        /// <summary>
        /// Render company header section
        /// </summary>
        protected void RenderCompanyHeader(IContainer container, ReportBrandingDTO branding)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    // Logo section
                    if (!string.IsNullOrWhiteSpace(branding.Logo))
                    {
                        row.ConstantItem(160).Element(logoContainer => RenderLogo(logoContainer, branding.Logo));
                        row.RelativeItem().PaddingLeft(20);
                    }
                    else
                    {
                        row.RelativeItem();
                    }

                    // Company info section
                    row.RelativeItem().Column(infoColumn =>
                    {
                        infoColumn.Item().Text(branding.Company.Name)
                            .FontSize(branding.Typography.HeaderSize)
                            .FontColor(branding.Colors.Primary)
                            .SemiBold();

                        if (!string.IsNullOrWhiteSpace(branding.Company.Address))
                        {
                            infoColumn.Item().PaddingTop(5).Text(branding.Company.Address)
                                .FontSize(branding.Typography.SmallSize)
                                .FontColor(branding.Colors.Text);
                        }

                        if (!string.IsNullOrWhiteSpace(branding.Company.Phone))
                        {
                            infoColumn.Item().Text($"Phone: {branding.Company.Phone}")
                                .FontSize(branding.Typography.SmallSize)
                                .FontColor(branding.Colors.Text);
                        }

                        if (!string.IsNullOrWhiteSpace(branding.Company.Email))
                        {
                            infoColumn.Item().Text($"Email: {branding.Company.Email}")
                                .FontSize(branding.Typography.SmallSize)
                                .FontColor(branding.Colors.Text);
                        }
                    });
                });
            });
        }

        /// <summary>
        /// Render standard footer with contact information
        /// </summary>
        protected void RenderFooter(IContainer container, ReportBrandingDTO branding)
        {
            container.AlignCenter().Text(text =>
            {
                text.DefaultTextStyle(TextStyle.Default
                    .FontSize(branding.Typography.SmallSize)
                    .FontColor(branding.Colors.Text));

                if (!string.IsNullOrWhiteSpace(branding.Company.Address))
                {
                    text.Span(branding.Company.Address);
                }

                if (!string.IsNullOrWhiteSpace(branding.Company.Phone))
                {
                    if (!string.IsNullOrWhiteSpace(branding.Company.Address))
                        text.Span(" | ");
                    text.Span($"Phone: {branding.Company.Phone}");
                }

                if (!string.IsNullOrWhiteSpace(branding.Company.Email))
                {
                    if (!string.IsNullOrWhiteSpace(branding.Company.Phone) || 
                        !string.IsNullOrWhiteSpace(branding.Company.Address))
                        text.Span(" | ");
                    text.Span($"Email: {branding.Company.Email}");
                }
            });
        }

        /// <summary>
        /// Create a styled table with alternating row colors
        /// </summary>
        protected void RenderStyledTable(IContainer container, List<string> headers, 
            List<List<string>> rows, BrandColorsDTO colors, TypographyDTO typography)
        {
            container.Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    for (int i = 0; i < headers.Count; i++)
                    {
                        columns.RelativeColumn();
                    }
                });

                // Header row
                foreach (var header in headers)
                {
                    table.Cell()
                        .Background(colors.Primary)
                        .Padding(8)
                        .Text(header)
                        .FontSize(typography.BodySize)
                        .FontColor(Colors.White)
                        .SemiBold();
                }

                // Data rows
                for (int i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    var isEvenRow = i % 2 == 0;
                    
                    foreach (var cell in row)
                    {
                        table.Cell()
                            .Background(isEvenRow ? Colors.White : colors.Secondary)
                            .Padding(8)
                            .Text(cell)
                            .FontSize(typography.BodySize)
                            .FontColor(colors.Text);
                    }
                }
            });
        }

        /// <summary>
        /// Parse and deserialize template data
        /// </summary>
        protected T? ParseTemplateData<T>(object data) where T : class
        {
            if (data is T directData)
                return directData;

            if (data is JsonElement jsonElement)
            {
                var jsonString = jsonElement.GetRawText();
                return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            var dataString = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<T>(dataString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        /// <summary>
        /// Format currency values
        /// </summary>
        protected string FormatCurrency(decimal amount, string currencyCode = "JOD", string culture = "en-US")
        {
            try
            {
                var cultureInfo = new CultureInfo(culture);
                return amount.ToString("N3", cultureInfo) + " " + currencyCode;
            }
            catch
            {
                return amount.ToString("N3") + " " + currencyCode;
            }
        }

        /// <summary>
        /// Format date values
        /// </summary>
        protected string FormatDate(DateTime date, string culture = "en-US")
        {
            try
            {
                var cultureInfo = new CultureInfo(culture);
                return date.ToString("dd/MM/yyyy", cultureInfo);
            }
            catch
            {
                return date.ToString("dd/MM/yyyy");
            }
        }

        #endregion
    }
}