using ReportHub.Objects.DTOs;
using ReportHub.Objects.HelperModel;

namespace ReportHub.Objects.Interfaces
{
    /// <summary>
    /// Template engine interface for managing and executing report templates
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// Register a template in the engine
        /// </summary>
        /// <param name="template">Template to register</param>
        void RegisterTemplate(IReportTemplate template);

        /// <summary>
        /// Get all registered templates
        /// </summary>
        /// <returns>Dictionary of template ID to template instance</returns>
        Dictionary<string, IReportTemplate> GetRegisteredTemplates();

        /// <summary>
        /// Check if a template is registered
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>True if template exists</returns>
        bool IsTemplateRegistered(string templateId);

        /// <summary>
        /// Get a specific template by ID
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Template instance or null</returns>
        IReportTemplate? GetTemplate(string templateId);

        /// <summary>
        /// Generate a report using the template engine
        /// </summary>
        /// <param name="request">Template report request</param>
        /// <returns>Generated report result</returns>
        Task<ReportResult> GenerateReportAsync(TemplateReportRequestDTO request);

        /// <summary>
        /// Validate a request against its template
        /// </summary>
        /// <param name="request">Request to validate</param>
        /// <returns>Validation result</returns>
        TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request);

        /// <summary>
        /// Get available templates with metadata
        /// </summary>
        /// <returns>Template metadata collection</returns>
        List<TemplateMetadata> GetAvailableTemplates();
    }

    /// <summary>
    /// Template metadata for discovery and documentation
    /// </summary>
    public class TemplateMetadata
    {
        public string TemplateId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public DateTime LastModified { get; set; }
        public bool IsActive { get; set; } = true;
        public TemplateConfigSchema ConfigSchema { get; set; } = new();
        public object SampleData { get; set; } = new();
    }
}