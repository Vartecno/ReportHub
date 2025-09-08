using QuestPDF.Infrastructure;
using ReportHub.Objects.DTOs;

namespace ReportHub.Objects.Interfaces
{
    /// <summary>
    /// Base interface for all report templates
    /// Provides the contract for template-based report generation
    /// </summary>
    public interface IReportTemplate
    {
        /// <summary>
        /// Unique identifier for the template
        /// </summary>
        string TemplateId { get; }

        /// <summary>
        /// Display name for the template
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Description of what this template generates
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Version of the template
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Supported data types for this template
        /// </summary>
        Type[] SupportedDataTypes { get; }

        /// <summary>
        /// Generate the report content using QuestPDF
        /// </summary>
        /// <param name="request">Template report request with data and branding</param>
        /// <returns>QuestPDF document container</returns>
        IContainer GenerateContent(TemplateReportRequestDTO request);

        /// <summary>
        /// Validate the request data for this template
        /// </summary>
        /// <param name="request">Request to validate</param>
        /// <returns>Validation result</returns>
        TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request);

        /// <summary>
        /// Get sample data structure for this template
        /// </summary>
        /// <returns>Sample request object</returns>
        object GetSampleData();

        /// <summary>
        /// Get configuration schema for this template
        /// </summary>
        /// <returns>Configuration schema information</returns>
        TemplateConfigSchema GetConfigSchema();
    }

    /// <summary>
    /// Template validation result
    /// </summary>
    public class TemplateValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    /// <summary>
    /// Template configuration schema
    /// </summary>
    public class TemplateConfigSchema
    {
        public string TemplateId { get; set; } = string.Empty;
        public Dictionary<string, FieldSchema> RequiredFields { get; set; } = new();
        public Dictionary<string, FieldSchema> OptionalFields { get; set; } = new();
        public Dictionary<string, object> DefaultValues { get; set; } = new();
        public List<string> SupportedCurrencies { get; set; } = new();
        public List<string> SupportedLanguages { get; set; } = new();
    }

    /// <summary>
    /// Field schema definition
    /// </summary>
    public class FieldSchema
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public object? DefaultValue { get; set; }
        public List<string>? AllowedValues { get; set; }
        public string? ValidationPattern { get; set; }
    }
}